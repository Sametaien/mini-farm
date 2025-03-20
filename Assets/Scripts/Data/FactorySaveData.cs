using System;

namespace Data
{
    [Serializable]
    public class FactorySaveData
    {
        public string factoryId;

        public int queueCount;

        public int deposit;

        public float remainingTime;

        public long lastUpdateTicks;
    }
}