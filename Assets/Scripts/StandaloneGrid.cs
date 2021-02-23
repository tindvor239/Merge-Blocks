using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class StandaloneGrid : Singleton<StandaloneGrid>
{
    [Header("Grid Size")]
    [SerializeField]
    private byte row = 7; //row of a grid
    [SerializeField]
    private byte column = 5;  // column of a grid

    private static Vector2 defaultPosition = new Vector2(0, 0.1f);
    private static float defaultSpace = 0.19f;
    private static float size = 1.3f;
    private byte lastRow = 0, lastColumn = 0;
    private List<GameObject> tiles;
    [SerializeField]
    private ObjectPool tilesPool = new ObjectPool("Tiles Pool");

    #region Properties
    public byte Row { get => row; }
    public byte Column { get => column; }
    #endregion

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
        SpawnGrid();
    }
    private void SpawnGrid()
    {
        if(lastRow != row || lastColumn != column)
        {
            ClearGrid();
            tiles = new List<GameObject>();
            for (byte row = 0; row < this.row; row++)
            {
                for(byte column = 0; column < this.column; column++)
                {
                    GameObject pooledObject = tilesPool.GetPooledObject();

                    if (pooledObject != null)
                    {
                        Debug.Log("In");
                        pooledObject.SetActive(true);
                    }
                    else
                    {
                        pooledObject = Instantiate(tilesPool.ObjectToPool, transform);
                        tilesPool.PooledObjects.Add(pooledObject);
                    }
                    pooledObject.transform.position = GetTilePosition(row, column);
                    pooledObject.transform.localScale = new Vector2(size, size);
                    tiles.Add(pooledObject);
                }
            }
            if (lastColumn != column)
                lastColumn = column;
            if (lastRow != row)
                lastRow = row;
        }
    }
    private void ClearGrid()
    {
        if(tiles != null)
        {
            foreach (GameObject tile in tiles)
            {
                if (tile != null)
                {
                    Debug.Log(tile);
                    tile.SetActive(false);
                }
            }
            tiles.Clear();
        }
    }
    private Vector2 GetTilePosition(int row, int column)
    {
        Vector2 result = new Vector2();
        result.x = ((column - (this.column / 2)) * defaultSpace) + defaultPosition.x;
        result.y = ((row - (this.row / 2)) * defaultSpace) + defaultPosition.y;
        return result;
    }
}
