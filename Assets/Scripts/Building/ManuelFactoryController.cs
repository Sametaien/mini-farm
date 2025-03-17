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
    public class ManualFactoryController : MonoBehaviour, IFactoryController
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

            if (_factoryModel.QueueCount.Value > 0) StartProduction().Forget();
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


        public void CollectOutput()
        {
            var amount = _factoryModel.Deposit.Value;
            if (amount <= 0) return;

            _resourceManager.AddResource(_factoryModel.OutputResource, amount);
            _factoryModel.Deposit.Value = 0;
        }

        [Inject]
        public void Construct(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        private async UniTaskVoid StartProduction()
        {
            if (_isProducing) return;
            _isProducing = true;

            while (_factoryModel.QueueCount.Value > 0)
            {
                var remain = _factoryModel.RemainingTime.Value;
                if (remain <= 0f)
                {
                    remain = _factoryModel.ProductionTime;
                    _factoryModel.RemainingTime.Value = remain;
                }

                while (remain > 0f && _factoryModel.QueueCount.Value > 0)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
                    var delta = Time.deltaTime;
                    remain -= delta;
                    if (remain < 0f) remain = 0f;

                    _factoryModel.RemainingTime.Value = remain;

                    if (_factoryModel.QueueCount.Value <= 0)
                        break;
                }

                if (_factoryModel.QueueCount.Value > 0)
                {
                    _factoryModel.Deposit.Value += 1;
                    _factoryModel.QueueCount.Value -= 1;
                    _factoryModel.RemainingTime.Value = 0f;
                }
            }

            _isProducing = false;
        }

        public void EnqueueProduction()
        {
            if (_factoryModel.IsFull.Value)
            {
                Debug.Log("[ManualFactory] Capacity is full. Cannot enqueue production.");
                return;
            }

            if (_factoryModel.InputResource != ResourceType.Null && _factoryModel.InputAmount > 0)
            {
                var success = _resourceManager.ConsumeResource(_factoryModel.InputResource, _factoryModel.InputAmount);
                if (!success)
                {
                    Debug.Log("[ManualFactory] Insufficient input resource to enqueue production.");
                    return;
                }
            }

            _factoryModel.QueueCount.Value++;

            if (!_isProducing) StartProduction().Forget();
        }

        public void CancelOneProduction()
        {
            if (_factoryModel.QueueCount.Value <= 0)
            {
                Debug.Log("[ManualFactory] There is no production to cancel.");
                return;
            }

            _factoryModel.QueueCount.Value--;

            // _resourceManager.AddResource(_factoryModel.InputResource, _factoryModel.InputAmount);
        }

        private void SaveFactoryData()
        {
            if (_factoryModel == null) return;

            var saveData = new FactorySaveData
            {
                factoryId = _factoryModel.FactoryId,
                queueCount = _factoryModel.QueueCount.Value,
                deposit = _factoryModel.Deposit.Value,
                remainingTime = _factoryModel.RemainingTime.Value,
                lastUpdateTicks = DateTime.Now.Ticks
            };

            SaveSystem.SaveFactory(saveData);
        }
    }
}