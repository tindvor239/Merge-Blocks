namespace UnityEngine.CustomComponents
{
    using System;
    public class Grid : Singleton<Grid>
    {
        [Header("Grid Size")]
        [SerializeField]
        private static byte row = 7; //row of a grid
        [SerializeField]
        private static byte column = 5;  // column of a grid

        [Header("Blocks")]
        [SerializeField]
        private Transform uIGrid;
        [SerializeField]
        private GameObject uIBlockprefabs;

        private static Block[,] masterBlocks = new Block[row, column];
        private static Transform[,] tiles = new Transform[row, column];

        private static Vector2 offset = new Vector2(-0.2f, -2.2f);
        private Block[,] lastBlocks = new Block[row, column];

        private static float defaultSpace = 1.1f;
        private static float defaultSize = 1.0f;

        #region Properties
        public static byte Row { get => row; }
        public static byte Column { get => column; }
        public static Block[,] MasterBlocks { get => masterBlocks; }
        public static Transform[,] Tiles { get => tiles; }
        public static float DefaultSpace { get => defaultSpace; set => defaultSpace = value; }
        public static float DefaultSize { get => defaultSize; set => defaultSize = value; }
        public static Vector2 Offset { get => offset; set => offset = value; }
        #endregion
        public enum GridState { sort, merge, normal }
        [SerializeField]
        public GridState gridState;
        // Awake is called when the script instance is being loaded.
        protected override void Awake()
        {
            for (int i = tiles.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = Instantiate(uIBlockprefabs, uIGrid).transform;
                }
            }
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
            if (Input.GetKey(KeyCode.B))
            {
                Debug.Log(DebugArray());
            }
        }
        public static string DebugArray()
        {
            string result = "";
            for (int row = Grid.row; row >= 0; row--)
            {
                if (row - 1 >= 0)
                    result += $"{row - 1}";
                else
                {
                    result += $" ";
                }
                for (int column = 0; column < Grid.column; column++)
                {
                    if (column == 0)
                    {
                        result += "     ";
                    }
                    if (row - 1 < 0)
                    {
                        result += $"  {column}";
                    }
                    else
                    {
                        if (row - 1 >= 0)
                        {
                            if (masterBlocks[row - 1, column] == null)
                            {
                                result += "  0";
                            }
                            else
                            {
                                result += $"  {masterBlocks[row - 1, column].Point}";
                            }
                        }
                    }
                    if (column == Grid.column - 1)
                    {
                        result += "\n";
                    }
                }
            }
            return result;
        }

        public static float GetLimitY(int column)
        {
            if (column > -1 && column < Grid.column)
            {
                for (byte row = 0; row < Grid.row; row++)
                {
                    if ((masterBlocks[row, column] == null))
                    {
                        return tiles[row, column].position.y;
                    }
                }
            }
            return -1;
        }
        public static float GetLimitYIgnoreCoin(int column)
        {
            if (column > -1 && column < Grid.column)
            {
                for (byte row = 0; row < Grid.row; row++)
                {
                    if ((masterBlocks[row, column] == null) || masterBlocks[row, column].GetComponent<Coin>())
                    {
                        return tiles[row, column].position.y;
                    }
                }
            }
            return -1;
        }
        public static float GetLimitY(int row, int column)
        {
            return tiles[row, column].position.y;
        }
        public static float GetLimitX(int column)
        {
            return tiles[0, column].position.x;
        }
        public static int GetColumnIndex(float positionX)
        {
            for (int column = 0; column < tiles.GetLength(1); column++)
            {
                if(Math.Round(tiles[0, column].position.x, 2) == Math.Round(positionX, 2))
                {
                    return column;
                }
            }
            return -1;
        }
        public static int GetRowIndex(float positionY)
        {
            for(int row = 0; row < tiles.GetLength(0); row++)
            {
                if(Math.Round(tiles[row, 0].position.y) == Math.Round(positionY))
                {
                    return row;
                }
            }
            return -1;
        }
        public static bool IsGridFull()
        {
            foreach (Block block in masterBlocks)
            {
                if (block == null)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsGridClean()
        {
            int count = 0;
            foreach(Block block in masterBlocks)
            {
                if(block == null)
                {
                    count++;
                }
            }
            if(count == masterBlocks.Length)
            {
                return true;
            }
            return false;
        }
    }
}