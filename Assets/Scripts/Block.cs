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
    private int currentRow;
    [SerializeField]
    private int currentColumn;
    [SerializeField]
    private int destinateRow, destinateColumn;
    private float limitY;

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
    public int DestinateRow { get => destinateRow; set => destinateRow = value; }
    public int DestinateColumn { get => destinateColumn; set => destinateColumn = value; }
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
        UpdatePosition(DestinateRow, DestinateColumn);
    }
    private void FixedUpdate()
    {
        if(isHit != true)
        {
            transform.position -= gravityMultiplier * Gravity * transform.up * Time.deltaTime;
        }
    }
    private void UpdatePosition(int row, int column)
    {
        limitY = Grid.Instance.GetRowPosition(row);
        if(transform.position.y <= limitY)
        {
            transform.position = new Vector2(transform.position.x, limitY);
            OnHit(row, column);
        }
    }
    private void OnHit(int row, int column)
    {
        currentRow = row;
        currentColumn = column;
        Grid.Instance.Blocks.Rows[row].Columns[column] = gameObject;

        int mergeAmount = GetAroundBlocksFromIndex(row, column);
        Debug.Log(row + ", " + column);
        bool canMerge = mergeAmount != 0 ? true : false;
        if (canMerge)
        {
            Debug.Log(mergeAmount);
            Merge(mergeAmount);
        }

        int rowBellow = IsBellowEmpty(row, column);
        if (rowBellow != -1)
        {
            //To do: if there isn't any block OR at the end of the grid.
            DestinateRow = rowBellow;
            Grid.Instance.Blocks.Rows[row].Columns[column] = null;
            isHit = false;
        }
        else
        {
            isHit = true;
            if (gameplay.ControlledBlock == this)
            {
                gameplay.onChangeBlock += ChangeBlock;
            }
        }

    }

    private void Merge(int amount)
    {
        // Get around blocks => int.
        // In Get around block => delete blocks.
        for(byte index = 0; index < amount; index++)
        {
            Point += Point;
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
                            if (currentObject != null && currentObject != gameObject && currentObject.GetComponent<Block>())
                            {
                                Block currentBlock = currentObject.GetComponent<Block>();

                                GameObject gameObject = Grid.Instance.Blocks.Rows[row].Columns[column];
                                if(gameObject != null && gameObject.GetComponent<Block>())
                                {
                                    if(currentBlock.Point == Point)
                                    {
                                        result++;
                                        PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(Grid.Instance.Blocks.Rows[rowIndex].Columns[columnIndex]);
                                        Debug.Log(Grid.Instance.Blocks.Rows[rowIndex].Columns[columnIndex]);
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
                        if (currentObject != null && currentObject != gameObject)
                        {
                            Block currentBlock = Grid.Instance.Blocks.Rows[rowIndex].Columns[column].GetComponent<Block>();
                            if (currentBlock && currentBlock.Point == Point)
                            {
                                result++;
                                PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(Grid.Instance.Blocks.Rows[rowIndex].Columns[column]);
                                Debug.Log(Grid.Instance.Blocks.Rows[rowIndex].Columns[column]);
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
        if (gameplay.ControlledBlock == this)
        {
            gameplay.onHitEvent += Uncontrolable;
        }
    }
    private int IsBellowEmpty(int row, int column)
    {
        int result = -1;
        if(row - 1 >= 0)
        {
            if(Grid.Instance.Blocks.Rows[row - 1].Columns[column] == null)
            {
                result = row - 1;
                return result;
            }
        }
        return result;
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
