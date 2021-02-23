using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gameplay : Singleton<Gameplay>
{
    private Snaper snaper;
    private Grid grid;

    [SerializeField]
    private GameObject blockPrefab = null;
    private Block currentBlock = null;

    public delegate void OnChangeBlock();
    public event OnChangeBlock onChangeBlock;
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
        if(blockPrefab != null)
        {
            currentBlock = Instantiate(blockPrefab).GetComponent<Block>();
            currentBlock.transform.position = Snaper.StartPosition;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            snaper.Snaping();
        }
    }

    private void LateUpdate()
    {
        currentBlock.transform.position = new Vector2(snaper.NearestPosition.x, currentBlock.transform.position.y);
        if(Input.GetMouseButton(0))
        {
            if(Input.GetMouseButtonUp(0))
            {
                //To do Block will be drop.
            }
        }
    }
}
