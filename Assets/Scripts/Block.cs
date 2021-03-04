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

    public static float slowGravityMultiplier = 0.5f, normalGravityMultiplier = 1f, fastGravityMultiplier = 1.2f;
    public float gravityMultiplier = slowGravityMultiplier;

    [SerializeField]
    private int currentRow;
    [SerializeField]
    private int currentColumn;
    [SerializeField]
    private int destinateRow, destinateColumn;
    private float limitY;

    private Gameplay gameplay;
    public int count = 0;
    private bool doneMerge = true;
    [SerializeField]
    private int mergeAmount;
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
    private void OnEnable()
    {
        mergeAmount = 0;
        count = 0;
        doneMerge = true;
    }
    private void Update()
    {
        UpdatePosition(DestinateRow, DestinateColumn);
        
        //bool canMerge = mergeAmount == count && count != 0 ? true : false;
        //if (canMerge)
        //{
        //    Debug.Log("In");
        //    Merge(mergeAmount);
        //    int rowBellow = IsBellowEmpty(DestinateRow, DestinateColumn);
        //    bool isBellowEmpty = rowBellow == -1 ? false : true;
        //    if (isBellowEmpty)
        //    {
        //        //To do: if there isn't any block OR at the end of the grid.
        //        DestinateRow = rowBellow;
        //        Grid.Instance.MasterBlocks.Rows[currentRow].Columns[DestinateColumn] = null;
        //        isHit = false;
        //        //DestinateRow = Grid.Instance.GetEmptyRowIndex(column);
        //    }
        //    else // block is at bottom of the grid or above a block.
        //    {
        //        mergeAmount = GetMergeBlocks(currentRow, currentColumn);
        //        Debug.Log(mergeAmount);
        //    }
        //    mergeAmount = 0;
        //    count = 0;
        //}
        //if (count == 0 && mergeAmount == 0)
        //{
        //    if(isHit)
        //    {
        //        if (gameplay.ControlledBlock == this)
        //        {
        //            gameplay.onChangeBlock += ChangeBlock;
        //        }
        //    }
        //}
    }
    private void FixedUpdate()
    {
        if(isHit == false)
        {
            transform.position -= gravityMultiplier * Gravity * transform.up * Time.deltaTime;
        }
    }
    private void UpdatePosition(int row, int column)
    {
        limitY = Grid.Instance.GetRowPosition(row);
        if(transform.position.y <= limitY && isHit == false)
        {
            transform.position = new Vector2(transform.position.x, limitY);
            OnHit(row, column);
        }
    }
    private void OnHit(int row, int column)
    {
        currentRow = row;
        currentColumn = column;
        Grid.Instance.MasterBlocks.Rows[row].Columns[column] = gameObject;
        count = 0;
        mergeAmount = GetMergeBlocks(row, column);
        Grid.Instance.onRearrangeGrid += Grid.Instance.RearrangeGrid;
        isHit = true;
        doneMerge = false;
    }
    private void RemoveDuplicate(int currentRow, int currentColumn)
    {
        for(byte row = 0; row < Grid.Instance.Row; row++)
        {
            if(row != currentRow)
            {
                if(Grid.Instance.MasterBlocks.Rows[row].Columns[currentColumn] == gameObject)
                {
                    Grid.Instance.MasterBlocks.Rows[row].Columns[currentColumn] = null;
                }
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
    private int GetMergeBlocks(int row, int column)
    {
        int result = 0;
        for (int rowIndex = row - 1; rowIndex <= row; rowIndex++)
        {
            if(rowIndex >= 0 && rowIndex < Grid.Instance.MasterBlocks.Rows.Count)
            {
                //Get Left Block and Right Block.
                if(rowIndex == row)
                {
                    for(int columnIndex = column - 1; columnIndex <= column + 1; columnIndex++)
                    {
                        if (columnIndex != column && columnIndex >= 0 && columnIndex < Grid.Instance.MasterBlocks.Rows[rowIndex].Columns.Count)
                        {
                            GameObject currentObject = Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[columnIndex];
                            if (currentObject != null && currentObject != gameObject && currentObject.GetComponent<Block>())
                            {
                                Block currentBlock = currentObject.GetComponent<Block>();

                                GameObject gameObject = Grid.Instance.MasterBlocks.Rows[row].Columns[column];
                                if(gameObject != null && gameObject.GetComponent<Block>())
                                {
                                    if(currentBlock.Point == Point)
                                    {
                                        result++;
                                        if(columnIndex > column)
                                        {
                                            Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[columnIndex].GetComponent<Block>().OnMoveLeft(transform.position.x, this);
                                        }
                                        else
                                        {
                                            Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[columnIndex].GetComponent<Block>().OnMoveRight(transform.position.x, this);
                                        }
                                        Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[columnIndex] = null;
                                    }
                                }
                            }
                        }
                    }
                }
                //Get Down Block.
                else
                {
                    if(column >= 0 && column < Grid.Instance.MasterBlocks.Rows[rowIndex].Columns.Count)
                    {
                        GameObject currentObject = Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[column];
                        if (currentObject != null && currentObject != gameObject)
                        {
                            Block currentBlock = Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[column].GetComponent<Block>();
                            if (currentBlock && currentBlock.Point == Point)
                            {
                                result++;
                                //Debug.Log(Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[column]);
                                Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[column].GetComponent<Block>().OnMoveUp(transform.position.y, this);
                                Grid.Instance.MasterBlocks.Rows[rowIndex].Columns[column] = null;
                            }
                        }
                    }
                }
            }
        }
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
        gravityMultiplier = fastGravityMultiplier;
        if (gameplay != null && gameplay.ControlledBlock != null && gameplay.ControlledBlock == this)
        {
            gameplay.onHitEvent += Uncontrolable;
        }
    }
    private int IsBellowEmpty(int row, int column)
    {
        int result = -1;
        if(row - 1 >= 0)
        {
            if(Grid.Instance.MasterBlocks.Rows[row - 1].Columns[column] == null)
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
        for (int row = Grid.Instance.MasterBlocks.Rows.Count - 1; row >= 0; row--)
        {
            for (int column = 0; column < Grid.Instance.MasterBlocks.Rows[row].Columns.Count; column++)
            {
                if (Grid.Instance.MasterBlocks.Rows[row].Columns[column] == gameObject)
                {
                    result[0] = row;
                    result[1] = column;
                    return result;
                }
            }
        }
        return result;
    }
    public void OnMoveUp(float destinationY, Block block)
    {
        StartCoroutine(MoveUpAndDestroy(destinationY, block));
    }
    IEnumerator MoveUpAndDestroy(float destinationY, Block block)
    {
        while(transform.position.y < destinationY)
        {
            transform.position += normalGravityMultiplier * Gravity * transform.up * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(transform.position.y >= destinationY)
        {
            transform.position = new Vector2(transform.position.x, destinationY);
            block.count++;
            //Debug.Log(block.count);
            //Debug.Log($"{block.name}");
            PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(gameObject);
        }
    }
    public void OnMoveLeft(float destinationX, Block block)
    {
        StartCoroutine(MoveLeftAndDestroy(destinationX, block));
    }
    IEnumerator MoveLeftAndDestroy(float destinationX, Block block)
    {
        while (transform.position.x > destinationX)
        {
            transform.position -= normalGravityMultiplier * Gravity * transform.right * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (transform.position.x <= destinationX)
        {
            transform.position = new Vector2(transform.position.x, destinationX);
            block.count++;
            Debug.Log("Done Move Left");
            PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(gameObject);
        }
    }
    public void OnMoveRight(float destinationX, Block block)
    {
        StartCoroutine(MoveRightAndDestroy(destinationX, block));
    }
    IEnumerator MoveRightAndDestroy(float destinationX, Block block)
    {
        while (transform.position.x < destinationX)
        {
            transform.position += normalGravityMultiplier * Gravity * transform.right * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (transform.position.x >= destinationX)
        {
            transform.position = new Vector2(transform.position.x, destinationX);
            block.count++;
            Debug.Log($"{name} Done Move Right");
            PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(gameObject);
        }
    }
    public void MoveDown(int destinationY)
    {
        isHit = false;
        DestinateRow = destinationY;
    }
}
