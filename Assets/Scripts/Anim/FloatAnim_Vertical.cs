using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatAnim_Vertical : MonoBehaviour
{
    private Tween tween;
    [SerializeField]
    private float height = 1f;
    [SerializeField]
    private float flyTime = 3f;
    [SerializeField]
    private float waitTime = 2f;
    

    private void OnEnable()
    {
        transform.localRotation = Quaternion.identity;
        Anim();
    }

    private void Anim()
    {
        tween = transform.DOLocalMoveY(transform.localPosition.y + height, flyTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            tween = transform.DOLocalMoveY(transform.localPosition.y, waitTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                tween = transform.DOLocalMoveY(transform.localPosition.y - height, flyTime).SetEase(Ease.Linear).OnComplete(() =>
                {
                    tween = transform.DOLocalMoveY(transform.localPosition.y,waitTime).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        Anim();
                    });
                });
            });
        });
    }

    private void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
        if (tween != null) tween.Kill();
    }
}
