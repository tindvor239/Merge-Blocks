using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Grid : Singleton<Grid>
{
    [Header("Grid Size")]
    [SerializeField]
    private byte row = 7; //row of a grid
    [SerializeField]
    private byte column = 5;  // column of a grid

    [Header("Blocks")]
    [SerializeField]
    private Serializable2DArray masterBlocks = null;
    private List<Block> movedBlocks = new List<Block>();
    public bool canSpawn = false;

    private static Vector2 defaultPosition = new Vector2(0, 0.1f);
    private static float defaultSpace = 1.1f;
    private static float defaultSize = 1.0f;
    private byte lastRow = 0, lastColumn = 0;

<<<<<<< Updated upstream
    public delegate void OnRearrangeGrid();
    public event OnRearrangeGrid onRearrangeGrid;

=======
    public delegate void OnSortTile();
    public event OnSortTile onSortTile;
>>>>>>> Stashed changes
    #region Properties
    public byte Row { get => row; }
    public byte Column { get => column; }
    public Serializable2DArray MasterBlocks { get => masterBlocks; }
    public bool IsFull { get => IsGridFull(); }
    public static Vector2 DefaultPosition { get => defaultPosition; set => defaultPosition = value; }
    public static float DefaultSpace { get => defaultSpace; set => defaultSpace = value; }
    public static float DefaultSize { get => defaultSize; set => defaultSize = value; } 
    #endregion

    // Awake is called when the script instance is being loaded.
    protected override void Awake()
    {
        #region Singleton
        #if UNITY_EDITOR
        base.Awake();
#endif
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
    }
    // Update is called once per frame
    private void Update()
    {
<<<<<<< Updated upstream
        if(onRearrangeGrid != null)
        {
            onRearrangeGrid.Invoke();
            onRearrangeGrid = null;
=======
        if(onSortTile != null)
        {
            onSortTile.Invoke();
            Debug.Log("Sorting");
            onSortTile = null;
>>>>>>> Stashed changes
        }
    }
    public void PlaceBlock(Vector2 position, GameObject block)
    {
        int column = Snaper.Instance.ConvertXToColumn(position.x);
        Debug.Log(column);
        int row = GetEmptyRowIndex(column);
        Debug.Log(row);
        masterBlocks.Rows[row].Columns[column] = block;
        movedBlocks.Add(block.GetComponent<Block>());
    }
    public void MergeAllBlocks()
    {
        //GET ALL MOVED BLOCKS AND MERGE IT!!
        //AFTER THAT SORT THE ARRAY
        //PLACE BLOCKS
        //ADD MOVED BLOCKS
        //MERGE BLOCKS
        //SORT THE ARRAY
    }
    public void RearrangeGrid()
    {
        for(int column = 0; column < this.column; column++)
        {
            if(isColumnArranged(column) == false)
            {
                MoveDown(column);
                Debug.Log("Not arrange yet");
            }
        }
        Debug.Log("All arranged");
            Gameplay.Instance.onChangeBlock += ChangeBlock;
    }
    private bool ChangeBlock()
    {
        return true;
    }
    private bool isColumnArranged(in int column)
    {
        int emptyRow = GetEmptyRowIndex(column);
        Debug.Log("empty row: "+ emptyRow);
        if (emptyRow != -1)
        {
            int row = GetBlockRowIndexFromTop(column);
            Debug.Log("row: " + row);
            if (row > emptyRow && row != -1)
            {
                Debug.Log("In");
                return false;
            }
        }
        return true;
    }
    private void MoveDown(in int column)
    {
        List<GameObject> moveBlocks = new List<GameObject>();
        for (byte row = 0; row < this.row; row++)
        {
            if (masterBlocks.Rows[row].Columns[column] != null)
            {
                moveBlocks.Add(masterBlocks.Rows[row].Columns[column]);
                masterBlocks.Rows[row].Columns[column] = null;
            }
        }
        int destinateRow = 0;
        foreach (GameObject block in moveBlocks)
        {
            if (masterBlocks.Rows[destinateRow].Columns[column] == null && destinateRow < this.row)
            {
                masterBlocks.Rows[destinateRow].Columns[column] = block;
                Debug.Log(block.name);
                block.GetComponent<Block>().MoveDown(destinateRow);
                row++;
            }
        }
    }
    public float GetRowPosition(int row)
    {
        float result = ((row - (this.row / 2)) * defaultSpace) + defaultPosition.y;
        return result;
    }
    public int GetPositionToRow(float y)
    {
        int row = Mathf.FloorToInt(((y - defaultPosition.y) + ((this.row / 2) * defaultSpace)) / defaultSpace);
        return row;
    }
    public bool IsGridFull()
    {
        foreach(RowOfObjects row in masterBlocks.Rows)
        {
            foreach(GameObject gameObject in row.Columns)
            {
                if(gameObject == null)
                {
                    return false;
                }
                else if(gameObject.GetComponent<Block>())
                {
                    Block block = gameObject.GetComponent<Block>();
                    if (block.IsHit == false)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public int GetEmptyRowIndex(int column)
    {
        for(byte row = 0; row < masterBlocks.Rows.Count; row++)
        {
            
            if(masterBlocks.Rows[row].Columns[column] == null)
            {
                return row;
            }
        }
        return -1;
    }
    private int GetBlockRowIndexFromTop(int column)
    {
        for(int row = this.row - 1; row >= 0; row--)
        {
            Debug.Log(column);
            Debug.Log(row);
            if(masterBlocks.Rows[row].Columns[column] != null)
            {
                return row;
            }
        }
        return -1;
    }
    private int[] GetBlockIndex(int row)
    {
        for(int column = 0; column < this.column; column++)
        {
            if(masterBlocks.Rows[row].Columns[column] != null)
            {
                int[] blockIndex = { row, column };
                return blockIndex;
            }
        }
        return new int[] { -1, -1 };
    }
}
