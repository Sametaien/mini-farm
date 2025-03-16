#region

using UnityEngine;

#endregion

namespace Data
{
    public static class SaveSystem
    {
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

        public static void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("[SaveSystem] Cleared all data");
        }
    }
}