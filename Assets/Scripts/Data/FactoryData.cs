#region

using UnityEngine;

#endregion

namespace Data
{
    public abstract class FactoryData : ScriptableObject
    {
        [Header("Factory Data")] public float productionTime;

        public int capacity;

        [Header("Resource Data")] public ResourceType inputResource;

        public ResourceType outputResource;
        public int inputAmount;
        public int outputAmount;
    }
}