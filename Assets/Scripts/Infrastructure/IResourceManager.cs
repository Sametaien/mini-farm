using System;
using Data;

namespace Infrastructure
{
    public interface IResourceManager
    {
        int GetResourceAmount(ResourceType resourceType);
        void AddResource(ResourceType resourceType, int amount);
        bool ConsumeResource(ResourceType resourceType, int amount);
        
        IObservable<(ResourceType resourceType, int newAmount)> OnResourceChanged { get; }
    }
}