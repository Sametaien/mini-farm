#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Data.Save
{
    public static class SaveSystem
    {
        private const string RESOURCES_KEY = "ResourcesData";


        public static void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("[SaveSystem] Cleared all data");
        }

        #region Factory

        public static void SaveFactory(FactorySaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            PlayerPrefs.SetString(saveData.factoryId, json);
            PlayerPrefs.Save();

            Debug.Log($"[SaveSystem] Saved factory {saveData.factoryId}");
        }

        public static FactorySaveData LoadFactory(string factoryId)
        {
            if (!PlayerPrefs.HasKey(factoryId)) return null;

            var json = PlayerPrefs.GetString(factoryId);
            var data = JsonUtility.FromJson<FactorySaveData>(json);
            Debug.Log($"[SaveSystem] Loaded factory {factoryId}");
            return data;
        }

        #endregion

        #region Resources

        public static void SaveResources(Dictionary<ResourceType, int> resources)
        {
            // ResourceManagerSave objesi oluşturalım
            var managerSave = new ResourceManagerSave();

            var list = new List<ResourceSaveData>();
            foreach (var kvp in resources)
                list.Add(new ResourceSaveData
                {
                    resourceType = kvp.Key,
                    amount = kvp.Value
                });
            managerSave.resources = list.ToArray();

            // JSON'a çevir
            var json = JsonUtility.ToJson(managerSave);
            PlayerPrefs.SetString(RESOURCES_KEY, json);
            PlayerPrefs.Save();

            Debug.Log($"[SaveSystem] Saved resources: {json}");
        }

        public static Dictionary<ResourceType, int> LoadResources()
        {
            if (!PlayerPrefs.HasKey(RESOURCES_KEY))
            {
                Debug.LogWarning("[SaveSystem] No saved resources found. Returning empty dictionary.");
                return new Dictionary<ResourceType, int>();
            }

            var json = PlayerPrefs.GetString(RESOURCES_KEY);
            var managerSave = JsonUtility.FromJson<ResourceManagerSave>(json);

            var dict = new Dictionary<ResourceType, int>();
            foreach (var resourceData in managerSave.resources) dict[resourceData.resourceType] = resourceData.amount;

            Debug.Log($"[SaveSystem] Loaded resources: {json}");
            return dict;
        }

        #endregion
    }
}