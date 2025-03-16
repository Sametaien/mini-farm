#region

using System;
using System.Collections.Generic;
using Infrastructure;
using UnityEngine;

#endregion

public abstract class ResourceManager : IResourceManager
{
    private readonly Dictionary<ResourceType, int> _resources;

    public ResourceManager()
    {
        _resources = new Dictionary<ResourceType, int>();
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType))) _resources.Add(resourceType, 0);
    }

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
        return true;
    }
}