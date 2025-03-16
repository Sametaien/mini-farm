using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MillSpinner : MonoBehaviour
{
    [Range(1, 15)] public float SpinSpeed = 10f;
    void Start()
    {
        transform.DOLocalRotate(new Vector3(0, 0, 360f), SpinSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}
