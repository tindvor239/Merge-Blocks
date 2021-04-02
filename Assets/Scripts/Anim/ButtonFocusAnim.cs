using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ButtonFocusAnim : MonoBehaviour
{
    private Tween tween;
    [SerializeField]
    [Range(0.1f, 2f)]
    private float waitTime = 1f;
    [SerializeField]
    [Range(1.1f, 2f)]
    private float scaleFactor = 1.1f;

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        Anim();
    }



    private void OnDisable()
    {
        transform.localScale = Vector3.one;
        if (tween != null) tween.Kill();
    }

    public void Anim()
    {
        tween = transform.DOScale(Vector3.one * scaleFactor, 0.2f).SetLoops(4, LoopType.Yoyo).OnComplete(() =>
        {
            tween = transform.DOScale(Vector3.one, waitTime).OnComplete(() =>
            {
                Anim();
            });
        });
    }
}
