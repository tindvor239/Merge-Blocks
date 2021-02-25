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

    public delegate void OnHitDelegate();
    public event OnHitDelegate onHitEvent;

    [SerializeField]
    private Block controlledBlock;
    private int currentColumn = 0;
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

        ChangeControlledBlock();
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
                if (Input.GetMouseButton(0))
                {
                    snaper.Snaping();
                    snaper.onSnaping += OnSnaping;
                }

                //This event will called once when controlling block's hit.
                if (onHitEvent != null)
                {
                    onHitEvent.Invoke();
                    ChangeControlledBlock();
                    onHitEvent = null;
                }

                if (ControlledBlock != null)
                {
                    RemoveDuplicateBlocks();
                    if (currentColumn != -1)
                    {
                        int row = ControlledBlock.UpdateBlockPosition(currentColumn);
                        if (row != -1)
                        {
                            grid.Blocks.Rows[row].Columns[currentColumn] = ControlledBlock.gameObject;
                        }
                    }
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
                if (ControlledBlock != null && ControlledBlock.IsHit == false)
                {
                    ControlledBlock.transform.position = new Vector2(snaper.NearestPosition.x, ControlledBlock.transform.position.y);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    //To do Block will be drop.
                    if (ControlledBlock != null)
                    {
                        ControlledBlock.Push();
                    }
                }
            break;
            #endregion
        }
    }
    private void OnSnaping(int index)
    {
        currentColumn = index;
    }
    private void RemoveDuplicateBlocks()
    {
        for (int row = Grid.Instance.Blocks.Rows.Count - 1; row >= 0; row--)
        {
            for (int column = 0; column < Grid.Instance.Blocks.Rows[row].Columns.Count; column++)
            {
                if (Grid.Instance.Blocks.Rows[row].Columns[column] != null && Grid.Instance.Blocks.Rows[row].Columns[column] == ControlledBlock.gameObject)
                {
                    Grid.Instance.Blocks.Rows[row].Columns[column] = null;
                }
            }
        }
    }
    private void ChangeControlledBlock()
    {
        //To do: spawn another block to control.
        controlledBlock = null;
        controlledBlock = GetControlledBlock().GetComponent<Block>();
        if(ControlledBlock != null)
        {
            ControlledBlock.transform.position = Snaper.StartPosition;
        }
    }
    //MUST BE RETURN TO GAMEOBJECT
    private GameObject GetControlledBlock()
    {
        ObjectPool objectPool = poolParty.GetPool("Blocks Pool");
        GameObject block = null;
        if (objectPool != null && objectPool.ObjectToPool != null)
        {
            if(objectPool.GetPooledObject() != null)
            {
                block = objectPool.GetPooledObject();
            }
            if(block == null)
            {
                block = objectPool.CreatePooledObject();
            }
            
        }
        return block;
    }
}
