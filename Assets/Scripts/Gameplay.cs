using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : Singleton<Gameplay>
{
    //Instances.
    private Snaper snaper;
    private Grid grid;
    private PoolParty poolParty;
    private GameManager gameManager;
    private bool controlable = false;

    public delegate bool OnHit();
    public event OnHit onHitEvent;

    public delegate bool OnChangeBlock();
    public event OnChangeBlock onChangeBlock;

    [SerializeField]
    private Block controlledBlock;
    #region Properties
    public Block ControlledBlock { get => controlledBlock; }
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
        snaper = Snaper.Instance;
        grid = Grid.Instance;
        poolParty = PoolParty.Instance;
        gameManager = GameManager.Instance;

        SpawnBlock();
    }

    // Update is called once per frame
    private void Update()
    {
        gameManager.onUpdate += OnUpdate;
    }

    private void LateUpdate()
    {
        gameManager.onUpdate += OnLateUpdate;
    }
    private void OnUpdate(GameManager.GameState state)
    {
        switch(state)
        {
            #region Play
            case GameManager.GameState.play:

                //Player controll.
                if(controlable)
                {
                    if (Input.GetMouseButton(0))
                    {
                        snaper.SwitchBlockPosition();
                    }
                }

                //This event will called once when controlling block's hit.
                if (onHitEvent != null)
                {
                    controlable = onHitEvent.Invoke();
                    //Debug.Log(controlable);
                    onHitEvent = null;
                }
                if (onChangeBlock != null)
                {
                    controlledBlock = null;
                    if (onChangeBlock.Invoke())
                    {
                        SpawnBlock();
                    }
                    onChangeBlock = null;
                }
                break;
            #endregion
        }
    }
    private void OnLateUpdate(GameManager.GameState state)
    {
        switch (state)
        {
            #region Play
            case GameManager.GameState.play:
                if(controlable)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        //To do Block will be drop.
                        if (ControlledBlock != null)
                        {
                            ControlledBlock.Push();
                        }
                    }
                }
            break;
            #endregion
        }
    }
    private void SpawnBlock()
    {
        //To do: spawn another block to control.
        controlledBlock = GetControlledBlock().GetComponent<Block>();
        controlledBlock.gravityMultiplier = Block.slowGravityMultiplier;
        snaper.SetBlockColumnOnSpawn();
        controlledBlock.transform.position = Snaper.StartPosition;
        controlable = true;
        //Debug.Log("In");
    }
    //MUST BE RETURN TO GAMEOBJECT
    private GameObject GetControlledBlock()
    {
        ObjectPool objectPool = poolParty.GetPool("Blocks Pool");
        GameObject pooledObject = null;
        if (objectPool != null && objectPool.ObjectToPool != null)
        {
            pooledObject = objectPool.GetPooledObject();
            if (pooledObject != null)
            {
                Block block = pooledObject.GetComponent<Block>();
                if(block != null)
                {
                    block.IsHit = false;
                }
            }
            if(pooledObject == null)
            {
                pooledObject = objectPool.CreatePooledObject();
            }
            
        }
        return pooledObject;
    }
    private bool IsEndTurn()
    {
        //If any nearby same point blocks.
        for(int row = 0; row < Grid.Instance.MasterBlocks.Rows.Count; row++)
        {
            for(int column = 0; column < Grid.Instance.MasterBlocks.Rows[row].Columns.Count; column++)
            {
                if(Grid.Instance.MasterBlocks.Rows[row].Columns[column] != null)
                {
                    GameObject currentBlock = Grid.Instance.MasterBlocks.Rows[row].Columns[column];
                    if (column + 1 < Grid.Instance.MasterBlocks.Rows[row].Columns.Count)
                    {
                        int nearbyColumn = column + 1;
                        if(Grid.Instance.MasterBlocks.Rows[row].Columns[nearbyColumn] != null)
                        {
                            GameObject nearbyObject = Grid.Instance.MasterBlocks.Rows[row].Columns[nearbyColumn];
                            if (nearbyObject.GetComponent<Block>() && currentBlock.GetComponent<Block>())
                            {
                                Block nearbyBlock = nearbyObject.GetComponent<Block>();
                                Block block = currentBlock.GetComponent<Block>();
                                if(nearbyBlock.Point == block.Point)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }
        //If any below same point blocks.
        for(int column = 0; column < Grid.Instance.MasterBlocks.Rows[0].Columns.Count; column++)
        {
            for(int row = 0; row < Grid.Instance.MasterBlocks.Rows.Count; row++)
            {
                if(Grid.Instance.MasterBlocks.Rows[row].Columns[column] != null)
                {
                    GameObject currentObject = Grid.Instance.MasterBlocks.Rows[row].Columns[column];
                    int belowRow = row + 1;
                    if(belowRow < Grid.Instance.MasterBlocks.Rows.Count)
                    {
                        if(Grid.Instance.MasterBlocks.Rows[belowRow].Columns[column] != null)
                        {
                            GameObject belowObject = Grid.Instance.MasterBlocks.Rows[belowRow].Columns[column];
                            if(belowObject.GetComponent<Block>() && currentObject.GetComponent<Block>())
                            {
                                Block belowBlock = belowObject.GetComponent<Block>();
                                Block currentBlock = currentObject.GetComponent<Block>();
                                if(belowBlock.Point == currentBlock.Point)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }
        return true;
    }
    private void OnControlledBlockMovingDown(int row)
    {

    }
}
