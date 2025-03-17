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

            // Sabit veriler
            ProductionTime = data.productionTime;
            Capacity = data.capacity;
            InputResource = data.inputResource;
            OutputResource = data.outputResource;
            InputAmount = data.inputAmount;
            OutputAmount = data.outputAmount;

            // Kaydedilmiş veri varsa yükleyelim
            if (saveData != null)
            {
                // Offline farkını hesaplamak için
                var lastDateTime = new DateTime(saveData.lastUpdateTicks);
                var offlineSeconds = (float)(DateTime.Now - lastDateTime).TotalSeconds;

                // Sıradaki emirler
                // (Örneğin eski 'currentProductionAmount' yerine artık 'queueCount' tutuyor olabilir)
                QueueCount = new ReactiveProperty<int>(saveData.queueCount);

                // Depodaki hazır ürün miktarı
                Deposit = new ReactiveProperty<int>(saveData.deposit);

                // Parçalı üretim süresi (devam eden cycle varsa)
                RemainingTime = new ReactiveProperty<float>(saveData.remainingTime);

                // Offline'da üretilebilecek emirleri hesaplayalım
                ApplyOfflineProduction(offlineSeconds);
            }
            else
            {
                // İlk defa
                QueueCount = new ReactiveProperty<int>(0);
                Deposit = new ReactiveProperty<int>(0);
                RemainingTime = new ReactiveProperty<float>(0f);
            }

            // Kapasite dolu mu?
            IsFull = new ReactiveProperty<bool>(QueueCount.Value >= Capacity);
            // QueueCount değişince IsFull güncelle
            QueueCount
                .Subscribe(value => { IsFull.Value = value >= Capacity; })
                .AddTo(_disposables);
        }

        // Kaç adet üretim emri var (manuel sipariş)
        public ReactiveProperty<int> QueueCount { get; }

        // Üretilmiş ama henüz toplanmamış ürün miktarı
        public ReactiveProperty<int> Deposit { get; }

        // Aktif üretimi tamamlamak için kalan süre
        public ReactiveProperty<float> RemainingTime { get; }

        // Kapasite doldu mu? (Yeni sipariş verilemez)
        public ReactiveProperty<bool> IsFull { get; }

        // ScriptableObject'ınızdan gelen sabit veriler
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

        /// <summary>
        ///     Oyun kapalıyken geçen süreyi kullanarak
        ///     queue'daki emirleri üretir, varsa deposu artar.
        ///     Basit yaklaşım: Her emir "ProductionTime" ister.
        /// </summary>
        private void ApplyOfflineProduction(float offlineSeconds)
        {
            // 1) Eğer kapatırken aktif üretim devam ediyorduysa (RemainingTime > 0)
            //    kapalı kalınan süre, bu üretimi tamamlayabilir mi?
            if (QueueCount.Value > 0 && RemainingTime.Value > 0f)
            {
                if (offlineSeconds >= RemainingTime.Value)
                {
                    // Bu cycle biter
                    Deposit.Value += 1; // 1 ürün üretildi
                    QueueCount.Value -= 1; // Sıradaki emir bitti
                    offlineSeconds -= RemainingTime.Value;
                    RemainingTime.Value = 0f; // Artık üretim kalmadı
                }
                else
                {
                    // Tam bitiremedi, kısmen ilerledi
                    RemainingTime.Value -= offlineSeconds;
                    offlineSeconds = 0f;
                }
            }

            // 2) Geriye kalan offline süreyle, sıradaki emirler üretilebilir mi?
            //    Her emir tam ProductionTime gerektirir.
            if (QueueCount.Value > 0)
            {
                // Tüm emirleri üretecek zaman var mı?
                var totalTimeNeeded = QueueCount.Value * ProductionTime;
                if (offlineSeconds >= totalTimeNeeded)
                {
                    // Tüm emirler üretilebilir
                    Deposit.Value += QueueCount.Value;
                    offlineSeconds -= totalTimeNeeded;
                    QueueCount.Value = 0;
                }
                else
                {
                    // Hepsi üretilemez, sadece bir kısmı
                    var producibleCount = (int)(offlineSeconds / ProductionTime);
                    Deposit.Value += producibleCount;
                    QueueCount.Value -= producibleCount;
                    offlineSeconds -= producibleCount * ProductionTime;

                    // Hâlâ Queue kalmışsa, son cycle kısmen ilerlemiştir.
                    if (QueueCount.Value > 0 && offlineSeconds > 0f)
                    {
                        // Yeni bir item için kısmen ilerleme
                        RemainingTime.Value = ProductionTime - offlineSeconds;
                        offlineSeconds = 0f;
                    }
                }
            }
        }
    }
}