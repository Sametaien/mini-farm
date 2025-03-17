#region

using Infrastructure;
using UnityEngine;
using Zenject;

#endregion

public class GameController : MonoBehaviour
{
    [Inject] private IResourceManager _resourceManager;

    private void OnApplicationQuit()
    {
        (_resourceManager as ResourceManager)?.SaveAllResources();
    }
}