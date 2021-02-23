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

    private Gameplay gameplay;
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
    public bool IsHit { get => isHit; }
    #endregion
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        gameplay = Gameplay.Instance;
    }
    private void FixedUpdate()
    {
        if(isHit != true)
        {
            rigidbody.position -= gravityMultiplier * Gravity * (Vector2)transform.up * Time.deltaTime;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //isHit = true;
        if(collision.tag != "Block")
        {
            Debug.Log(collision.gameObject.name);
            isHit = true;
            gameplay.onHitEvent += OnHit;
            gravityMultiplier = slowGravityMultiplier;
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Serializable2DArray tiles = StandaloneGrid.Instance.Tiles;
    //    Serializable2DArray blocks = Gameplay.Instance.Blocks;
    //    for (int row = tiles.Rows.Count - 1; row >= 0; row--)
    //    {
    //        for (int column = tiles.Rows[row].Columns.Count - 1; column >= 0; column--)
    //        {
    //            if(tiles.Rows[row].Columns[column] == collision.gameObject && blocks.Rows[row].Columns[column] == null)
    //            {
    //                Debug.Log($"hit at\nRow: {row}, Column: {column} ");
    //            }
    //        }
    //    }

    //}
    public void Push()
    {
        gravityMultiplier = normalGravityMultiplier;
    }

    private void OnHit()
    {
        gameplay.controlledBlock = null;
    }
}
