using UnityEngine;
using System.Collections.Generic;

public class Grid : Singleton<Grid>
{
    //[SerializeField]
    //private byte row = 7; //row of a grid
    //[SerializeField]
    //private byte column = 5;  // column of a grid
    //[SerializeField]
    //private GameObject tilePrefab;
    //private byte lastRow = 7;
    //private byte lastColumn = 5;


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
        
    }

    // OnValidate is called once when value is changed in inspector
    private void OnValidate()
    {
        #region Unity Editor
        #if UNITY_EDITOR
        OnInspectorFieldsChanged();
        #endif
        #endregion
    }

    private void OnInspectorFieldsChanged()
    {
        //if (row != lastRow)
        //{
        //    //Debug.Log("row is changed");
        //    lastRow = row;
        //}
        //if (column != lastColumn)
        //{
        //    //Debug.Log("column is changed");
        //    column = lastColumn;
        //}
    }
}
