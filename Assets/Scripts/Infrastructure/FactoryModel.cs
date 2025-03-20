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

                QueueCount = new ReactiveProperty<int>(saveData.queueCount);

                Deposit = new ReactiveProperty<int>(saveData.deposit);

                RemainingTime = new ReactiveProperty<float>(saveData.remainingTime);

                ApplyOfflineProduction(offlineSeconds);
            }
            else
            {
                QueueCount = new ReactiveProperty<int>(0);
                Deposit = new ReactiveProperty<int>(0);
                RemainingTime = new ReactiveProperty<float>(0f);
            }

            IsQueueFull = new ReactiveProperty<bool>(false);
            IsDepositFull = new ReactiveProperty<bool>(false);
            QueueCount
                .Subscribe(q => { IsQueueFull.Value = q >= Capacity; })
                .AddTo(_disposables);

            Deposit
                .Subscribe(d => { IsDepositFull.Value = d >= Capacity; })
                .AddTo(_disposables);
        }

        public ReactiveProperty<int> QueueCount { get; }

        public ReactiveProperty<int> Deposit { get; }

        public ReactiveProperty<float> RemainingTime { get; }

        public ReactiveProperty<bool> IsQueueFull { get; }
        public ReactiveProperty<bool> IsDepositFull { get; }

        public float ProductionTime { get; }
        public int Capacity { get; }
        public ResourceType InputResource { get; }
        public ResourceType OutputResource { get; }
        public int InputAmount { get; }
        public int OutputAmount { get; }

        public string FactoryId { get; }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void ApplyOfflineProduction(float offlineSeconds)
        {
            if (QueueCount.Value > 0 && RemainingTime.Value > 0f)
            {
                if (offlineSeconds >= RemainingTime.Value)
                {
                    Deposit.Value += 1;
                    QueueCount.Value -= 1;
                    offlineSeconds -= RemainingTime.Value;
                    RemainingTime.Value = 0f;
                }
                else
                {
                    RemainingTime.Value -= offlineSeconds;
                    offlineSeconds = 0f;
                }
            }

            if (QueueCount.Value > 0)
            {
                var totalTimeNeeded = QueueCount.Value * ProductionTime;
                if (offlineSeconds >= totalTimeNeeded)
                {
                    Deposit.Value += QueueCount.Value;
                    offlineSeconds -= totalTimeNeeded;
                    QueueCount.Value = 0;
                }
                else
                {
                    var producibleCount = (int)(offlineSeconds / ProductionTime);
                    Deposit.Value += producibleCount;
                    QueueCount.Value -= producibleCount;
                    offlineSeconds -= producibleCount * ProductionTime;

                    if (QueueCount.Value > 0 && offlineSeconds > 0f)
                    {
                        RemainingTime.Value = ProductionTime - offlineSeconds;
                        offlineSeconds = 0f;
                    }
                }
            }
        }
    }
}