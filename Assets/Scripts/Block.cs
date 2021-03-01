using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private TextMesh point = null;
    //will be unserialize.
    [SerializeField]
    private bool isHit;

    public static float slowGravityMultiplier = 0.01f, normalGravityMultiplier = 1f;
    public float gravityMultiplier = slowGravityMultiplier;

    [SerializeField]
    private int currentColumn;
    [SerializeField]
    private int currentRow;

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
    public bool IsHit
    {
        get
        {
            return isHit;
        }
         set => isHit = value; }
    #endregion
    private void Awake()
    {
    }
    private void Start()
    {
        gameplay = Gameplay.Instance;
        currentRow = Grid.Instance.Blocks.Rows.Count - 1;
    }
    private void Update()
    {
        Snaper.Instance.onSwitchingColumn += OnSwitchingColumn;

        int row = UpdateBlockPosition(currentColumn);
        if (currentColumn != -1 && row != -1)
        {
            currentRow = row;
            RemoveDuplicateBlock();
            Grid.Instance.Blocks.Rows[currentRow].Columns[currentColumn] = gameObject;
        }
    }
    private void FixedUpdate()
    {
        if(isHit != true)
        {
            transform.position -= gravityMultiplier * Gravity * transform.up * Time.deltaTime;
        }
    }
    private void RemoveDuplicateBlock()
    {
        for (int row = Grid.Instance.Blocks.Rows.Count - 1; row >= 0; row--)
        {
            for (int column = 0; column < Grid.Instance.Blocks.Rows[row].Columns.Count; column++)
            {
                if (Grid.Instance.Blocks.Rows[row].Columns[column] != null && Grid.Instance.Blocks.Rows[row].Columns[column] == this.gameObject)
                {
                    Grid.Instance.Blocks.Rows[row].Columns[column] = null;
                }
            }
        }
    }
    private void OnSwitchingColumn(int column)
    {
        if(gameplay.ControlledBlock == this)
        {
            currentColumn = column;
        }
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
                //Get Left Block and Right Block.
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
                //Get Up Block and Down Block.
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
                if(Grid.Instance.Blocks.Rows[row].Columns[column] != null && Grid.Instance.Blocks.Rows[row].Columns[column] == gameObject)
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
    private void OnHit(int row, int column)
    {
        //IsHit = true;
        Merge();
        bool isBellowEmpty = IsBellowEmpty(row, column);
        if (gameplay.ControlledBlock == this)
        {
            gameplay.onHitEvent += Uncontrolable;
        }
        if (isBellowEmpty)
        {
            //To do: if there isn't any block OR at the end of the grid.
        }
        else
        {
            isHit = true;
            if(gameplay.ControlledBlock == this)
            {
                gameplay.onChangeBlock += ChangeBlock;
            }
        }

    }
    private bool ChangeBlock()
    {
        return true;
    }
    private bool Uncontrolable()
    {
        return false;
    }
    public void Push()
    {
        gravityMultiplier = normalGravityMultiplier;
    }
    private bool IsBellowEmpty(int row, int column)
    {
        if(row - 1 >= 0)
        {
            if(Grid.Instance.Blocks.Rows[row - 1].Columns[column] != null)
            {
                return false;
            }
        }
        else if(row == 0)
        {
            if(Grid.Instance.Blocks.Rows[row].Columns[column] != null)
            {
                return false;
            }
        }
        return true;
    }
    private int UpdateBlockPosition(int column)
    {
        int result = 0;
        if(IsHit == false && column != -1)
        {
            //for (int row = currentRow; row >= 0; row--)
            //{
            if(currentRow - 1 >= 0)
            {
                int row = currentRow - 1;
                Debug.Log("In1");
                float rowPosition = Grid.Instance.GetRowPosition(row);
                if(Grid.Instance.Blocks.Rows[row].Columns[column] == null)
                {
                    // return top row where block position is greater than the top of the grid.
                    if (row >= Grid.Instance.Row - 1 && transform.position.y >= rowPosition)
                    {
                        Debug.Log("In2");
                        result = row;
                        return result;
                    }
                    // return row where block position is in middle of the grid.
                    else if (transform.position.y >= rowPosition && row + 1 < Grid.Instance.Row && transform.position.y < Grid.Instance.GetRowPosition(row + 1))
                    {
                        Debug.Log("In3");
                        result = row;
                        return result;
                    }
                    // return row where block is at bottom of the grid.
                    if (row == 0)
                        Debug.Log("Yea");
                    if (row <= 0 && transform.position.y <= rowPosition)
                    {
                        Debug.Log("In4");
                        transform.position = new Vector2(transform.position.x, rowPosition);
                        OnHit(row, column);
                        return result;
                    }
                }
                else
                {
                    //get index of block that bellow it.
                    if(row - 1 >= 0 && Grid.Instance.Blocks.Rows[row - 1].Columns[column] != null)
                    {
                        Debug.Log("In5");
                        //get the index above current index.
                        //set position block follow the current row.
                        if (transform.position.y <= rowPosition)
                        {
                            Debug.Log("In6");
                            transform.position = new Vector2(transform.position.x, rowPosition);
                            OnHit(row, column);
                            result = row;
                            return result;
                        }
                    }
                }
            }
            //}
        }
        //else
        //{
        //    for(int row = 0; row < Grid.Instance.Blocks.Rows.Count; row++)
        //    {
        //        if(Grid.Instance.Blocks.Rows[row].Columns[column] == gameObject)
        //        {
        //            Debug.Log("In");
        //            if(row - 1 >= 0 && Grid.Instance.Blocks.Rows[row - 1].Columns[column] == null)
        //            {
        //                isHit = false;
        //            }
        //            result = row;
        //            return result;
        //        }
        //    }
        //}
        Debug.Log($"{gameObject} column{column}");
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
    private bool isOnControl()
    {
        return false;
    }
}
