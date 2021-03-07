using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [Header("Controlling Block")]
    [SerializeField]
    private Block controllingBlock;
    [SerializeField]
    private List<Block> movedBlocks = new List<Block>();
    [SerializeField]
    private List<Block> mergedBlocks = new List<Block>();
    [SerializeField]
    private Dictionary<Block, Vector2> shiftBlocks = new Dictionary<Block, Vector2>();
    public Dictionary<Block, Vector2> ShiftBlocks { get => shiftBlocks; }
    public List<Block> MergedBlock { get => mergedBlocks; }
    private float lastX = 0;
    private float limitY = 0;
    public static bool controlable = false;

    private bool isHit = false;
    private bool doneShift = false;
    private bool doneMove = false;

    public delegate void OnChangeBlock();
    public event OnChangeBlock onChangeBlock;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if(controllingBlock != null)
        {
            if(Input.GetMouseButton(0) && controlable)
            {
                int x = limitX();
                limitY = Grid.Instance.GetLimitY(x);
                lastX = x * Grid.DefaultSpace;
                controllingBlock.transform.position = new Vector2(lastX, controllingBlock.transform.position.y);
            }
            if(Input.GetMouseButtonUp(0))
            {
                controllingBlock.Push();
                controlable = false;
            }
            if(controllingBlock.transform.position.y > limitY)
            {
                controllingBlock.MoveDown();
            }
            else
            {
                controllingBlock.transform.position = new Vector2(controllingBlock.transform.position.x, limitY);
                lastX = controllingBlock.transform.position.x;
                PlaceBlock(controllingBlock);
                movedBlocks.Add(controllingBlock);
                controllingBlock = null;
                isHit = true;
            }
        }
        else
        {
            if(isHit && doneShift == false)
            {
                MereAll();
            }
            if(mergedBlocks.Count == 0 && doneMove == false && doneShift)
            {
                foreach(KeyValuePair<Block, Vector2> pair in shiftBlocks)
                {
                    pair.Key.Move(pair.Value);
                }
                doneMove = true;
            }
            if(shiftBlocks.Count == 0 && doneMove && doneShift)
            {
                if(movedBlocks.Count == 0)
                {
                    SpawnBlock();
                    doneMove = false;
                    doneShift = false;
                    isHit = false;
                }
                else
                {
                    doneMove = false;
                    doneShift = false;
                }
            }
        }

    }
    private void SpawnBlock()
    {
        ObjectPool pool = PoolParty.Instance.GetPool("Blocks Pool");
        GameObject pooledObject = pool.GetPooledObject();
        if(pooledObject == null)
        {
            pooledObject = pool.CreatePooledObject();
            Debug.Log("PooledObject");
        }
        limitY = Grid.Instance.GetLimitY((int)(lastX / Grid.DefaultSpace));
        controllingBlock = pooledObject.GetComponent<Block>();
        controllingBlock.gravityMultiplier = Block.slowGravityMultiplier;
        controlable = true;
        controllingBlock.transform.position = new Vector2(lastX, 8f);
    }
    private int limitX()
    {
        float x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        int roundedX = Mathf.FloorToInt(x);
        if(roundedX <= 0)
        {
            return 0;
        }
        else if(roundedX >= Grid.Column - 1)
        {
            return Grid.Column - 1;
        }
        else
        {
            int roundedY = Mathf.FloorToInt(controllingBlock.transform.position.y);
            if(roundedY > Grid.Row - 1)
            {
                roundedY = Grid.Row - 1;
            }
            float y = roundedY * Grid.DefaultSpace;
            if(Grid.MasterBlocks[roundedY, roundedX] == null)
            {
                return roundedX;
            }
            else
            {
                for(int i = 0; i < Grid.Row; i++)
                {
                    if(Grid.MasterBlocks[roundedY, i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }
    public static void PlaceBlock(Block block)
    {
        int column = Mathf.FloorToInt(block.transform.position.x / Grid.DefaultSpace);
        int row = Mathf.FloorToInt(block.transform.position.y / Grid.DefaultSpace);
        Grid.MasterBlocks[row, column] = block;
    }
    private void MereAll()
    {
        List<Block> notMergeBlocks = new List<Block>();
        if(movedBlocks.Count != 0)
        {
            foreach(Block block in movedBlocks)
            {
                List<Block> neighbours = GetNeighbours(block);
                if (neighbours != null && neighbours.Count != 0)
                {
                    Merge(block, neighbours);
                }
                else
                {
                    if(!notMergeBlocks.Contains(block))
                    notMergeBlocks.Add(block);
                }
            }
            foreach(Block block in notMergeBlocks)
            {
                Debug.Log(block);
                movedBlocks.Remove(block);
            }
            ShiftDown();
        }
        doneShift = true;
    }
    private void ShiftDown()
    {
        for(int column = 0; column < Grid.Column; column++)
        {
            for(int row = 0; row < Grid.Row; row++)
            {
                if(Grid.MasterBlocks[row, column] != null)
                {
                    Debug.Log(Grid.MasterBlocks[row, column]);
                    if (row > 0)
                    {
                        for(int i = 0; i < row; i++)
                        {
                            if(Grid.MasterBlocks[i, column] == null)
                            {
                                Grid.MasterBlocks[i, column] = Grid.MasterBlocks[row, column];
                                Grid.MasterBlocks[row, column] = null;
                                float y =  i* Grid.DefaultSpace;
                                //movedown.
                                if(shiftBlocks.ContainsKey(Grid.MasterBlocks[i, column]))
                                {
                                    shiftBlocks.Remove(Grid.MasterBlocks[i, column]);
                                }
                                shiftBlocks.Add(Grid.MasterBlocks[i, column], new Vector2(column * Grid.DefaultSpace, y));
                                if (!movedBlocks.Contains(Grid.MasterBlocks[i, column]))
                                {
                                    movedBlocks.Add(Grid.MasterBlocks[i, column]);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    private void Merge(Block block, List<Block> neighbours)
    {
        foreach(Block neighbour in neighbours)
        {
            block.Point *= 2;
        }
    }
    private int[] GetIndex(Block block)
    {
        if(block != null)
        {
            for(int row = 0; row < Grid.Row; row++)
            {
                for(int column = 0; column < Grid.Column; column++)
                {
                    if(Grid.MasterBlocks[row, column] != null && Grid.MasterBlocks[row, column].gameObject == block.gameObject)
                    {
                        return new int[] { row, column};
                    }
                }
            }
        }
        return new int[] { -1, -1 };
    }
    private List<Block> GetNeighbours(Block block)
    {
        List<Block> result = new List<Block>();
        int[] indexes = GetIndex(block);
        bool indexesAvailable = indexes[0] != -1 ? true : false;
        if(indexesAvailable)
        {
            int row = indexes[0], column = indexes[1];
            Block leftBlock = GetLeft(block, row, column);
            if(leftBlock != null)
            {
                result.Add(leftBlock);
            }

            Block rightBlock = GetRight(block, row, column);
            if (rightBlock != null)
            {
                result.Add(rightBlock);
            }

            Block belowBlock = GetBelow(block, row, column);
            if (belowBlock != null)
            {
                result.Add(belowBlock);
            }
            return result;
        }
        return null;
    }
    private Block GetLeft(Block block, int row, int column)
    {
        if(column - 1 >= 0 && Grid.MasterBlocks[row, column - 1] != null && Grid.MasterBlocks[row, column - 1].Point == block.Point)
        {
            Block leftBlock = Grid.MasterBlocks[row, column - 1];
            mergedBlocks.Add(leftBlock);
            leftBlock.Merge(block.transform.position);
            Grid.MasterBlocks[row, column - 1] = null;
            return leftBlock;
        }
        return null;
    }
    private Block GetRight(Block block, int row, int column)
    {
        if (column + 1 < Grid.Column && Grid.MasterBlocks[row, column + 1] != null && Grid.MasterBlocks[row, column + 1].Point == block.Point)
        {
            Block rightBlock = Grid.MasterBlocks[row, column + 1];
            mergedBlocks.Add(rightBlock);
            rightBlock.Merge(block.transform.position);
            Grid.MasterBlocks[row, column + 1] = null;
            return rightBlock;
        }
        return null;
    }
    private Block GetBelow(Block block, int row, int column)
    {
        if (row - 1 >= 0 && Grid.MasterBlocks[row - 1, column] != null && Grid.MasterBlocks[row - 1, column].Point == block.Point)
        {
            Block belowBlock = Grid.MasterBlocks[row - 1, column];
            mergedBlocks.Add(belowBlock);
            belowBlock.Merge(block.transform.position);
            Grid.MasterBlocks[row - 1, column] = null;
            return belowBlock;
        }
        return null;
    }
    private float GetAvailableColumn()
    {
        for (byte column = (byte)(Grid.Column - 1); column >= 0; column--)
        {
            for (byte row = (byte)(Grid.Row - 1); row >= 0; row--)
            {
                if (Grid.MasterBlocks[row, column] == null)
                {
                    lastX = column * Grid.DefaultSpace;
                    return row * Grid.DefaultSpace;
                }
            }
        }
        return -1;
    }
}
