#region

using System;
using System.Collections.Generic;
using Data;
using UniRx;
using UnityEngine;

#endregion

namespace Infrastructure
{
    public class ResourceManager : IResourceManager
    {
        private readonly Subject<(ResourceType resourceType, int newAmount)> _onResourceChanged = new();
        private readonly Dictionary<ResourceType, int> _resources;

        public ResourceManager()
        {
            _resources = SaveSystem.LoadResources();
            
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                _resources.TryAdd(resourceType, 0);
            }
        }

        public IObservable<(ResourceType resourceType, int newAmount)> OnResourceChanged
            => _onResourceChanged;

        public int GetResourceAmount(ResourceType resourceType)
        {
            return _resources.GetValueOrDefault(resourceType, 0);
        }

        public void AddResource(ResourceType resourceType, int amount)
        {
            if (!_resources.ContainsKey(resourceType)) _resources[resourceType] = 0;

            _resources[resourceType] += amount;
            Debug.Log($"[ResourceManager] Added {amount} {resourceType} to resources" +
                      $" (total: {_resources[resourceType]})");

            _onResourceChanged.OnNext((resourceType, _resources[resourceType]));
        }

        public bool ConsumeResource(ResourceType resourceType, int amount)
        {
            if (!_resources.ContainsKey(resourceType) || _resources[resourceType] < amount)
            {
                Debug.LogWarning($"Not enough {resourceType} to consume!");
                return false;
            }

            _resources[resourceType] -= amount;
            Debug.Log($"[ResourceManager] Consumed {amount} {resourceType} from resources" +
                      $" (total: {_resources[resourceType]})");

            _onResourceChanged.OnNext((resourceType, _resources[resourceType]));
            return true;
        }
        
        public void SaveAllResources()
        {
            SaveSystem.SaveResources(_resources);
        }
    }
}