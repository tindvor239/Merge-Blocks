﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : Singleton<Gameplay>
{
    [Header("Blocks")]
    [SerializeField]
    private Serializable2DArray blocks = null;

    private Snaper snaper;
    private StandaloneGrid grid;
    private PoolParty poolParty;

    public delegate void OnHitDelegate();
    public event OnHitDelegate onHitEvent;

    #region Properties
    public Block controlledBlock { get; set; }
    public Serializable2DArray Blocks { get => blocks; }
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
        grid = StandaloneGrid.Instance;
        poolParty = PoolParty.Instance;

        ChangeControlledBlock();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            snaper.Snaping();
            snaper.onSnaping += OnSnaping;
        }

        //This event will called once when event is called.
        if(onHitEvent != null)
        {
            onHitEvent.Invoke();
            ChangeControlledBlock();
            onHitEvent = null;
        }
    }

    private void LateUpdate()
    {
        if(controlledBlock != null && controlledBlock.IsHit == false)
        {
            controlledBlock.transform.position = new Vector2(snaper.NearestPosition.x, controlledBlock.transform.position.y);
        }
        if(Input.GetMouseButtonUp(0))
        {
            //To do Block will be drop.
            if(controlledBlock != null)
            {
                controlledBlock.Push();
            }
        }
    }

    private void OnSnaping(int index)
    {
        for(int row = 0; row < blocks.Rows.Count; row++)
        {
            for(int column = 0; column < blocks.Rows[row].Columns.Count; column++)
            {
                if (blocks.Rows[row].Columns[column] == controlledBlock.gameObject)
                {
                    Debug.Log($"get it self \nrow: {row}, column: {column}");
                    blocks.Rows[row].Columns[column] = null;
                }
            }
        }
        blocks.Rows[0].Columns[index] = controlledBlock.gameObject;
        Debug.Log($"index: {index}");
    }
    private void ChangeControlledBlock()
    {
        //To do: spawn another block to control.
        controlledBlock = CreateControlledBlock();
        if(controlledBlock != null)
        {
            controlledBlock.transform.position = Snaper.StartPosition;
        }
    }
    private Block CreateControlledBlock()
    {
        ObjectPool objectPool = poolParty.GetPool("Blocks Pool");
        if (objectPool != null && objectPool.ObjectToPool != null)
        {
            Block block = objectPool.CreatePooledObject().GetComponent<Block>();
            if (block != null)
            {
                return block;
            }
        }
        return null;
    }
}
