using UnityEngine;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask buildingLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingLayer))
            {
                Building building = hit.transform.GetComponent<Building>();
                if (building != null)
                {
                    building.Highlight();
                }
            }
        }
    }
}