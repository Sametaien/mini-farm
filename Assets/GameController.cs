#region

using System;
using Infrastructure;
using UnityEngine;
using Zenject;

#endregion

public class GameController : MonoBehaviour
{
    [Inject] private IResourceManager _resourceManager;

    private void Awake()
    {
        Application.targetFrameRate = 120;
    }

    private void OnApplicationQuit()
    {
        (_resourceManager as ResourceManager)?.SaveAllResources();
    }
}