#region

using System;

#endregion

namespace Data
{
    [Serializable]
    public class ResourceSaveData
    {
        public ResourceType resourceType;
        public int amount;
    }

    [Serializable]
    public class ResourceManagerSave
    {
        public ResourceSaveData[] resources;
    }
}