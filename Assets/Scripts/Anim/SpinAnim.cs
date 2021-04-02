using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SpinAnim : MonoBehaviour
{
    private Tween tween;
    [Range(0.1f, 2f)]
    private float time = 2f;

    private void OnEnable()
    {
        transform.localRotation = Quaternion.identity;
        tween = transform.DOLocalRotate(Vector3.forward * 180, time).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
        if (tween != null) tween.Kill();
    }
}
