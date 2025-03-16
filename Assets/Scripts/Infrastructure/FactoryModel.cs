#region

using System;
using Data;
using UniRx;

#endregion

namespace Infrastructure
{
    public class FactoryModel : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();

        public FactoryModel(string factoryId, FactoryData data, FactorySaveData saveData = null)
        {
            FactoryId = factoryId;

            ProductionTime = data.productionTime;
            Capacity = data.capacity;
            InputResource = data.inputResource;
            OutputResource = data.outputResource;
            InputAmount = data.inputAmount;
            OutputAmount = data.outputAmount;

            if (saveData != null)
            {
                var lastDateTime = new DateTime(saveData.lastUpdateTicks);
                var offlineSeconds = (float)(DateTime.Now - lastDateTime).TotalSeconds;

                CurrentProductionAmount = new ReactiveProperty<int>(saveData.currentProductionAmount);

                var remain = saveData.currentProductionTime - offlineSeconds;
                if (remain < 0f)
                    remain = 0f;

                RemainingTime = new ReactiveProperty<float>(remain);
            }
            else
            {
                CurrentProductionAmount = new ReactiveProperty<int>(0);
                RemainingTime = new ReactiveProperty<float>(ProductionTime);
            }

            IsFull = new ReactiveProperty<bool>(CurrentProductionAmount.Value >= Capacity);

            CurrentProductionAmount
                .Subscribe(_ => { IsFull.Value = CurrentProductionAmount.Value >= Capacity; })
                .AddTo(_disposables);
        }

        public ReactiveProperty<float> RemainingTime { get; private set; }
        public ReactiveProperty<int> CurrentProductionAmount { get; }
        public ReactiveProperty<bool> IsFull { get; }

        public float ProductionTime { get; }
        public int Capacity { get; }
        public ResourceType InputResource { get; }
        public ResourceType OutputResource { get; }
        public int InputAmount { get; }
        public int OutputAmount { get; }

        public string FactoryId { get; private set; }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}