using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh = null;
    [SerializeField]
    private bool isHit;

    private static float slowGravityMultiplier = 0.01f, normalGravityMultiplier = 1f;
    private static float gravityMultiplier = slowGravityMultiplier;
    private new Rigidbody2D rigidbody;
    #region Properties
    public static float Gravity { get => 9.81f; }
    public uint Point
    {
        get
        {
            if(textMesh != null)
            {
                try
                {
                    return uint.Parse(textMesh.text);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
    #endregion
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if(isHit != true)
        {
            rigidbody.position -= gravityMultiplier * Gravity * (Vector2)transform.up * Time.deltaTime;
        }
    }
    public void Push()
    {
        gravityMultiplier = normalGravityMultiplier;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        isHit = true;
        gravityMultiplier = slowGravityMultiplier;
    }
}
