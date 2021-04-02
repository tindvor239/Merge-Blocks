using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FloatAnim_Horizontal : MonoBehaviour
{
    private Tween tween;
    [SerializeField]
    private float width = 30f;
    [SerializeField]
    private float flyTime = 3f;
    [SerializeField]
    private float waitTime = 2f;
    [SerializeField]
    private int timeBeforeWait = 2;

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
        tween = transform.DOLocalMoveX(transform.localPosition.x + currentWidth * 2, flyTime).OnComplete(() =>
        {
            tween = transform.DOLocalMoveX(transform.localPosition.x - currentWidth * 2, flyTime).OnComplete(() =>
            {
                if(time >= timeBeforeWait)
                {
                    tween = transform.DOLocalMoveX(transform.localPosition.x, waitTime).OnComplete(() =>
                    {
                        time = 0;
                        Anim();
                    });
                }
                else
                {
                    Anim();
                    time++;
                }
            });
        });
    }

    private void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
        if (tween != null) tween.Kill();
    }
}
