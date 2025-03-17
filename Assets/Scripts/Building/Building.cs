#region

using DG.Tweening;
using UnityEngine;

#endregion

namespace Building
{
    public class Building : MonoBehaviour
    {
        public GameObject buildingUICanvas;
        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = transform.localScale;
            buildingUICanvas.SetActive(false);
        }

        public void Highlight()
        {
            Debug.Log($"Building clicked via centralized input: {name}");

            transform.DOKill(); // Stop any previous tween

            transform.localScale = _initialScale;

            transform.DOScale(_initialScale * 1.1f, 0.2f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
        }

        public void ShowBuildingUI()
        {
            if (buildingUICanvas != null && !buildingUICanvas.activeSelf)
                buildingUICanvas.SetActive(true);
        }

        public void CollectResources()
        {
            if (TryGetComponent<IFactoryController>(out var factoryController))
                factoryController.CollectOutput();
            else
                Debug.LogWarning("[Building] No IFactoryController found on building.");
        }
    }
}