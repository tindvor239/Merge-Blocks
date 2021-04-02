using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MoveBackAndForwardAnim : MonoBehaviour
{
    private Tween tween;
    [SerializeField]
    private float width = 2f;
    [SerializeField]
    private float flyTime = 3f;


    private int time = 0;
    [HideInInspector]
    public float currentWidth;
    private void OnEnable()
    {
        transform.localRotation = Quaternion.identity;
        Vector3 pos = transform.localPosition;
        pos.x -= width;
        currentWidth = width;
        transform.localPosition = pos;
        Anim();
    }

    public void StopAnim()
    {
        currentWidth = 0;
    }

    public void RestartAnim()
    {
        currentWidth = width;
    }

    private void Anim()
    {
        time++;
        tween = transform.DOLocalMoveX(transform.localPosition.x + currentWidth, flyTime).OnComplete(() =>
        {
            tween = transform.DOLocalMoveX(transform.localPosition.x - currentWidth, flyTime).OnComplete(() =>
            {
                Anim();
            });
        });
    }

    private void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
        if (tween != null) tween.Kill();
    }
}
