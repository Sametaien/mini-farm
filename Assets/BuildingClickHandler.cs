#region

using DG.Tweening;
using UnityEngine;

#endregion

public class BuildingClickHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log("Mouse Down");
        transform.DOScale(0.65f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);
    }
}