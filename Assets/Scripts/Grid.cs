using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class Grid : Singleton<Grid>
{
    [Header("Grid Size")]
    [SerializeField]
    private static byte row = 7; //row of a grid
    [SerializeField]
    private static byte column = 5;  // column of a grid

    [Header("Blocks")]
    [SerializeField]
    private static Block[,] masterBlocks = new Block[row,column];

    private Block[,] lastBlocks = new Block[row, column];

    private static float defaultSpace = 1.1f;
    private static float defaultSize = 1.0f;

    #region Properties
    public static byte Row { get => row; }
    public static byte Column { get => column; }
    public static Block[,] MasterBlocks { get => masterBlocks; }
    public static float DefaultSpace { get => defaultSpace; set => defaultSpace = value; }
    public static float DefaultSize { get => defaultSize; set => defaultSize = value; }
    #endregion
    public enum GridState { sort, merge, normal}
    [SerializeField]
    public GridState gridState;
    // Awake is called when the script instance is being loaded.
    protected override void Awake()
    {
        #region Singleton
        base.Awake();
        #endregion
    }

    // Start is called before the first frame update
    private void Start()
    {
    }
    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKey(KeyCode.B))
        {
            Debug.Log(DebugArray());
        }
    }
    public string DebugArray()
    {
        string result = "";
        for(int row = Grid.row; row >= 0; row--)
        {
            if(row - 1 >= 0)
                result += $"{row - 1}";
            else
            {
                result += $" ";
            }
            for(int column = 0; column < Grid.column; column++)
            {
                if(column == 0)
                {
                    result += "     ";
                }
                if(row - 1 < 0)
                {
                    result += $"  {column}";
                }
                else
                {
                    if(row - 1 >= 0)
                    {
                        if(masterBlocks[row - 1, column] == null)
                        {
                            result += "  0";
                        }
                        else
                        {
                            result += $"  {masterBlocks[row - 1, column].Point}";
                        }
                    }
                }
                if(column == Grid.column - 1)
                {
                    result += "\n";
                }
            }
        }
        return result;
    }
    public float GetLimitY(int column)
    {
        if (column > -1 && column < Grid.column)
        {
            for(byte row = 0; row < Grid.row; row++)
            {
                if((masterBlocks[row, column] == null))
                    return row * defaultSpace;
            }
        }
        return -1;
    }
}
