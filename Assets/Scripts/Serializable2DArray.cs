using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Serializable2DArray
{
    private List<RowOfObjects> rows;
    #region Properties
    public List<RowOfObjects> Rows;
    #endregion
    #region Constructor
    public Serializable2DArray()
    {
        rows = new List<RowOfObjects>();
    }
    public Serializable2DArray(int rowRange)
    {
        rows = new RowOfObjects[rowRange].ToList();
    }
    public Serializable2DArray(int rowRange, int columnRange)
    {
        RowOfObjects[] rows = new RowOfObjects[rowRange];
        this.rows = rows.ToList();
        for (int i = 0; i < rowRange; i++)
        {
            RowOfObjects row = new RowOfObjects(columnRange);
            row.name = i.ToString();
            this.rows.Add(row);
        }
    }
    #endregion
}

[Serializable]
public class RowOfObjects
{
    [SerializeField]
    private List<GameObject> columns;
    #region Properties
    public string name { get; set; }
    public List<GameObject> Columns { get => columns; }
    #endregion
    #region Constructor
    public RowOfObjects()
    {
        columns = new List<GameObject>();
    }
    public RowOfObjects(int range)
    {
        columns = new List<GameObject>();
        columns = new GameObject[range].ToList();
    }
    #endregion
}
