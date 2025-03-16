using DG.Tweening;
using UnityEngine;

public class Building : MonoBehaviour
{
    private Vector3 _initialScale;

    void Awake()
    {
        _initialScale = transform.localScale;
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
}
