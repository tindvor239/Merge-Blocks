using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private TextMesh point = null;
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
            if(point != null)
            {
                try
                {
                    return uint.Parse(point.text);
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

        private set
        {
            point.text = value.ToString();
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
        //if(isHit != false && Gameplay.Instance.ControlledBlock != this)
        //{
        //    int[] initPosition = GetInit();
        //    int row = initPosition[0], column = initPosition[1];
        //    if(row != -1 && column != -1)
        //    {
        //        UpdateBlockPosition(initPosition[1]);
        //    }
        //}
        //else if(isHit && Gameplay.Instance.ControlledBlock != this)
        //{
        //    int[] initPosition = GetInit();
        //    int row = initPosition[0], column = initPosition[1];
        //    if (IsBelowEmpty(row, column))
        //    {
        //        isHit = false;
        //    }
        //}
    }
    private void FixedUpdate()
    {
        if(isHit != true)
        {
            transform.position -= gravityMultiplier * Gravity * transform.up * Time.deltaTime;
        }
    }
    private bool IsBelowEmpty(int row, int column)
    {
        if(column - 1 >= 0 && Grid.Instance.Blocks.Rows[row].Columns[column - 1] == null)
        {
            return true;
        }
        return false;
    }
    private void Merge()
    {
        int[] initPosition = GetInit();
        int row = initPosition[0], column = initPosition[1];
        // Get around blocks => int.
        // In Get around block => delete blocks.
        if (row != -1 && column != -1)
        {
            int amount = GetAroundBlocksFromIndex(row, column);
            for(byte index = 0; index < amount; index++)
            {
                Point += Point;
            }
        }
    }
    private int GetAroundBlocksFromIndex(int row, int column)
    {
        int result = 0;

        for (int rowIndex = row - 1; rowIndex <= row + 1; rowIndex++)
        {
            if(rowIndex >= 0 && rowIndex < Grid.Instance.Blocks.Rows.Count)
            {
                if(rowIndex == row)
                {
                    for(int columnIndex = column - 1; columnIndex <= column + 1; columnIndex++)
                    {
                        if (columnIndex != column && columnIndex >= 0 && columnIndex < Grid.Instance.Blocks.Rows[rowIndex].Columns.Count)
                        {
                            GameObject currentObject = Grid.Instance.Blocks.Rows[rowIndex].Columns[columnIndex];
                            if (currentObject != null && currentObject.GetComponent<Block>())
                            {
                                Block currentBlock = currentObject.GetComponent<Block>();

                                GameObject gameObject = Grid.Instance.Blocks.Rows[row].Columns[column];
                                if(gameObject != null && gameObject.GetComponent<Block>())
                                {
                                    if(currentBlock.Point == Point)
                                    {
                                        result++;
                                        PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(Grid.Instance.Blocks.Rows[rowIndex].Columns[columnIndex]);
                                        Grid.Instance.Blocks.Rows[rowIndex].Columns[columnIndex] = null;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(column >= 0 && column < Grid.Instance.Blocks.Rows[rowIndex].Columns.Count)
                    {
                        GameObject currentObject = Grid.Instance.Blocks.Rows[rowIndex].Columns[column];
                        if (currentObject != null)
                        {
                            Block currentBlock = Grid.Instance.Blocks.Rows[rowIndex].Columns[column].GetComponent<Block>();
                            if (currentBlock && currentBlock.Point == Point)
                            {
                                result++;
                                PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(Grid.Instance.Blocks.Rows[rowIndex].Columns[column]);
                                Grid.Instance.Blocks.Rows[rowIndex].Columns[column] = null;
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
    private int[] GetInit()
    {
        int[] result = new int[2];
        for(byte row = 0; row < Grid.Instance.Blocks.Rows.Count; row++)
        {
            for (byte column = 0; column < Grid.Instance.Blocks.Rows[row].Columns.Count; column++)
            {
                if(Grid.Instance.Blocks.Rows[row].Columns[column] == gameObject)
                {
                    result[0] = row;
                    result[1] = column;
                    return result;
                }
            }
        }
        result[0] = -1;
        result[1] = -1;
        return result;
    }
    private void OnHit()
    {
        //Merge();
    }
    public void Push()
    {
        gravityMultiplier = normalGravityMultiplier;
    }
    public int UpdateBlockPosition(int column)
    {
        int result = 0;
        if(isHit == false)
        {
            for (int row = Grid.Instance.Blocks.Rows.Count - 1; row >= 0; row--)
            {
                float rowPosition = Grid.Instance.GetRowPosition(row);
                if(Grid.Instance.Blocks.Rows[row].Columns[column] == null)
                {
                    // return top row where block position is greater than the top of the grid.
                    if (row >= Grid.Instance.Row - 1 && transform.position.y >= rowPosition)
                    {
                        result = (byte)row;
                        return result;
                    }
                    // return row where block position is in middle of the grid.
                    else if (row + 1 < Grid.Instance.Row && transform.position.y >= rowPosition && transform.position.y <= Grid.Instance.GetRowPosition(row + 1))
                    {
                        result = (byte)row;
                        return result;
                    }
                    // return row where block is at bottom of the grid.
                    else if (row <= 0 && transform.position.y <= rowPosition)
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
                    if(Grid.Instance.Blocks.Rows[row].Columns[column] != gameObject)
                    {
                        byte currentRow = (byte)row;
                        //get the index above current index.
                        //currentRow = row + 1;
                        if(currentRow + 1 < Grid.Instance.Blocks.Rows.Count)
                        {
                            currentRow += 1;
                            rowPosition = Grid.Instance.GetRowPosition(currentRow);
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
        return -1;
    }
    public int[] GetCurrentIndex()
    {
        int[] result = new int[2];
        for (int row = Grid.Instance.Blocks.Rows.Count - 1; row >= 0; row--)
        {
            for (int column = 0; column < Grid.Instance.Blocks.Rows[row].Columns.Count; column++)
            {
                if (Grid.Instance.Blocks.Rows[row].Columns[column] == gameObject)
                {
                    result[0] = row;
                    result[1] = column;
                    return result;
                }
            }
        }
        return result;
    }
}
