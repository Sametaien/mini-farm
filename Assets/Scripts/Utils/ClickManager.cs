#region

using UnityEngine;
using UnityEngine.EventSystems;

#endregion


namespace Utils
{
    public class ClickManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask buildingLayer;

        private Building.Building _selectedBuilding;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 100f, buildingLayer))
            {
                if (hit.collider.TryGetComponent<Building.Building>(out var clickedBuilding))
                {
                    if (_selectedBuilding != null && _selectedBuilding != clickedBuilding)
                        _selectedBuilding.HideBuildingUI();

                    _selectedBuilding = clickedBuilding;

                    _selectedBuilding.Highlight();
                    _selectedBuilding.ShowBuildingUI();
                    _selectedBuilding.CollectResources();
                }
            }
            else
            {
                if (_selectedBuilding == null) return;
                _selectedBuilding.HideBuildingUI();
                _selectedBuilding = null;
            }
        }
    }
}