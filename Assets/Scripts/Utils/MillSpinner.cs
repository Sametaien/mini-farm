#region

using DG.Tweening;
using UnityEngine;

#endregion

namespace Utils
{
    public class MillSpinner : MonoBehaviour
    {
        [Range(1, 15)] public float spinSpeed = 10f;

        private void Start()
        {
            transform.DOLocalRotate(new Vector3(0, 0, 360f), spinSpeed, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear);
        }
    }
}