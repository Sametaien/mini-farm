using System;

namespace Data
{
    [Serializable]
    public class FactorySaveData
    {
        public string factoryId;

        // Kaç emir var
        public int queueCount;

        // Depodaki üretilmiş ürün
        public int deposit;

        // Üretimi devam eden sipariş için kalan süre
        public float remainingTime;

        // Son kayıt zamanı (offline için)
        public long lastUpdateTicks;
    }
}