#region

using UnityEngine;

#endregion

namespace Utils
{
    public class ClickManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask buildingLayer;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100f, buildingLayer))
            {
                var buildingObject = hit.collider.TryGetComponent(out Building.Building building);
                if (buildingObject) building.Highlight();
            }
        }
    }
}