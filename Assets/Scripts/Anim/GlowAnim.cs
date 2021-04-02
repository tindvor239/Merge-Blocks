using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class GlowAnim : MonoBehaviour
{
    private Tween tween;
    private SpriteRenderer sr;

    public float time = 1f;
    public float fadeFactor = 0.5f;



    private void OnEnable()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        transform.localRotation = Quaternion.identity;
        Color color = sr.color;
        color.a = 1f;
        sr.color = color;
        Anim();
    }

    private void Anim()
    {
        tween = sr.DOFade(fadeFactor, time).SetLoops(-1, LoopType.Yoyo);
      
    }

    private void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
        if (tween != null) tween.Kill();
    }
}
