#region

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Infrastructure;
using UI;
using UnityEngine;
using Zenject;

#endregion

namespace Building
{
    public class FactoryController : MonoBehaviour, IFactoryController
    {
        [Header("Factory Data")] public FactoryData factoryData;

        [Header("UI Elements")] public InfoSlider progressSlider;

        private CancellationTokenSource _cancellationTokenSource;

        private FactoryModel _factoryModel;
        private bool _isProducing;
        private IResourceManager _resourceManager;

        private void OnEnable()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var factoryId = gameObject.name;

            var saveData = SaveSystem.LoadFactory(factoryId);

            _factoryModel = new FactoryModel(factoryId, factoryData, saveData);

            progressSlider.SetFactoryModel(_factoryModel);

            StartProduction().Forget();
        }

        private void OnDisable()
        {
            _cancellationTokenSource.Cancel();

            SaveFactoryData();

            _factoryModel?.Dispose();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
                SaveFactoryData();
        }

        private void OnApplicationQuit()
        {
            SaveFactoryData();
        }

        [Inject]
        public void Construct(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }


        private async UniTaskVoid StartProduction()
        {
            _isProducing = true;

            while (_isProducing)
            {
                if (_factoryModel.IsFull.Value)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
                    continue;
                }

                if (_factoryModel.InputResource != ResourceType.Null && _factoryModel.InputAmount > 0)
                {
                    var success = _resourceManager.ConsumeResource(_factoryModel.InputResource,
                        _factoryModel.InputAmount);
                    if (!success)
                    {
                        await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
                        continue;
                    }
                }

                var remain = _factoryModel.RemainingTime.Value;

                while (remain > 0f && !_factoryModel.IsFull.Value)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);

                    var deltaTime = Time.deltaTime;
                    remain -= deltaTime;
                    if (remain < 0f) remain = 0f;

                    _factoryModel.RemainingTime.Value = remain;

                    if (_cancellationTokenSource.IsCancellationRequested)
                        return;
                }

                _factoryModel.Deposit.Value += _factoryModel.OutputAmount;

                if (!_factoryModel.IsFull.Value) _factoryModel.RemainingTime.Value = _factoryModel.ProductionTime;
            }
        }


        public void CollectOutput()
        {
            var amount = _factoryModel.Deposit.Value;
            if (amount <= 0) return;

            _resourceManager.AddResource(_factoryModel.OutputResource, amount);
            _factoryModel.Deposit.Value = 0;
        }


        private void SaveFactoryData()
        {
            if (_factoryModel == null) return;

            var saveData = new FactorySaveData
            {
                factoryId = _factoryModel.FactoryId,
                remainingTime = _factoryModel.RemainingTime.Value,
                deposit = _factoryModel.Deposit.Value,
                lastUpdateTicks = DateTime.Now.Ticks
            };

            SaveSystem.SaveFactory(saveData);
        }
    }
}