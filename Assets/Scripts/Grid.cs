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
    private Serializable2DArray blocks = null;

    private static Vector2 defaultPosition = new Vector2(0, 0.1f);
    private static float defaultSpace = 0.19f;
    private static float defaultSize = 1.3f;
    private byte lastRow = 0, lastColumn = 0;

    #region Properties
    public byte Row { get => row; }
    public byte Column { get => column; }
    public Serializable2DArray Blocks { get => blocks; }
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
        
    }

    // OnValidate is called once when value is changed in inspector
    private void OnValidate()
    {
        #region Unity Editor
        OnEditor();
        #endregion
    }

    private void OnEditor()
    {
        RowOfObjects[] rows = new RowOfObjects[row];
        GameObject[] columns = new GameObject[column];

        //blocks.Rows = rows.ToList();
        //for(int index = 0; index < blocks.Rows.Count; index++)
        //{
        //    blocks.Rows[index] = new RowOfObjects();
        //    blocks.Rows[index].Columns = columns.ToList();
        //}
    }
    public float GetRowPosition(int row)
    {
        float result = ((row - (this.row / 2)) * defaultSpace) + defaultPosition.y;
        return result;
    }
    public bool IsGridFull()
    {
        foreach(RowOfObjects row in blocks.Rows)
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
        int result = -1;
        for(byte row = 0; row < blocks.Rows.Count; row++)
        {
            
            if(blocks.Rows[row].Columns[column] == null)
            {
                result = row;
                return result;
            }
        }
        return result;
    }
}
