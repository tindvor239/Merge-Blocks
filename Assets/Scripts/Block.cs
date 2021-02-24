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
    }
    private void Start()
    {
        gameplay = Gameplay.Instance;
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(isHit != true)
        {
            transform.position -= gravityMultiplier * Gravity * transform.up * Time.deltaTime;
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
    public byte UpdateBlockPosition(byte column)
    {
        byte result = 0;
        if(isHit == false)
        {
            for (int row = Gameplay.Instance.Blocks.Rows.Count - 1; row >= 0; row--)
            {
                float rowPosition = StandaloneGrid.Instance.GetRowPosition(row);
                if(Gameplay.Instance.Blocks.Rows[row].Columns[column] == null)
                {
                    // return top row where block position is greater than the top of the grid.
                    if (row >= StandaloneGrid.Instance.Row - 1 && transform.position.y >= rowPosition)
                    {
                        result = (byte)row;
                        return result;
                    }
                    // return row where block position is in middle of the grid.
                    else if (row - 1 >= 0 && transform.position.y <= rowPosition && transform.position.y >= StandaloneGrid.Instance.GetRowPosition(row - 1))
                    {
                        result = (byte)row;
                        return result;
                    }
                    // return row where block is at bottom of the grid.
                    if (row <= 0)
                    {
                        transform.position = new Vector2(transform.position.x, rowPosition);

                        isHit = true;
                        gameplay.onHitEvent += OnHit;
                        gravityMultiplier = slowGravityMultiplier;
                        return result;
                    }
                }
                else
                {
                    //get index of block that isn't not itself.
                    if(Gameplay.Instance.Blocks.Rows[row].Columns[column] != gameObject)
                    {
                        byte currentRow = (byte)row;
                        //get the index above current index.
                        //currentRow = row + 1;
                        if(currentRow + 1 < Gameplay.Instance.Blocks.Rows.Count)
                        {
                            currentRow += 1;
                            rowPosition = StandaloneGrid.Instance.GetRowPosition(currentRow);
                        }
                        //set position block follow the current row.
                        if(transform.position.y <= rowPosition)
                        {
                            transform.position = new Vector2(transform.position.x, rowPosition);
                            isHit = true;
                            gameplay.onHitEvent += OnHit;
                            gravityMultiplier = slowGravityMultiplier;
                            result = currentRow;
                            return result;
                        }
                    }
                }
            }
        }
        return result;
    }
    public int[] GetCurrentIndex()
    {
        int[] result = new int[2];
        for (int row = Gameplay.Instance.Blocks.Rows.Count - 1; row >= 0; row--)
        {
            for (int column = 0; column < Gameplay.Instance.Blocks.Rows[row].Columns.Count; column++)
            {
                if (Gameplay.Instance.Blocks.Rows[row].Columns[column] == gameObject)
                {
                    result[0] = row;
                    result[1] = column;
                    return result;
                }
            }
        }
        return result;
    }
    private void OnHit()
    {
        gameplay.controlledBlock = null;
    }
}
