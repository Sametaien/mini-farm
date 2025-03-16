#region

using System;

#endregion

namespace Data
{
    [Serializable]
    public class FactorySaveData
    {
        public string factoryId;
        public float currentProductionTime;
        public int currentProductionAmount;
        public long lastUpdateTicks;
    }
}