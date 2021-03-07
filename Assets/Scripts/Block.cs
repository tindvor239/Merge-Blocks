using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;

    public static float normalGravityMultiplier = 1.0f, slowGravityMultiplier = 0.1f, fastGravityMultiplier = 1.2f;
    public float gravityMultiplier = slowGravityMultiplier;

    private Vector2 destination = new Vector2();
    public delegate void OnMergeDelegate();
    public event OnMergeDelegate onMerge;

    public delegate void OnMoveDelegate();
    public event OnMergeDelegate onMove;
    #region Properties
    public int Point
    {
        get
        {
            try
            {
                return int.Parse(textMesh.text);
            }
            catch
            {
                return 0;
            }
        }
        set => textMesh.text = value.ToString();
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(onMerge != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, normalGravityMultiplier * 9.82f * Time.deltaTime);
            onMerge.Invoke();
        }
        if(onMove != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, normalGravityMultiplier * 9.82f * Time.deltaTime);
            onMove.Invoke();
        }
    }
    private void FixedUpdate()
    {
    }
    public void Move(Vector2 destination)
    {
        this.destination = destination;
        onMove += OnMove;
    }
    public void Merge(Vector2 destination)
    {
        this.destination = destination;
        onMerge += OnMerge;
    }
    private void OnMove()
    {
        if((Vector2)transform.position == destination)
        {
            GameController.Instance.ShiftBlocks.Remove(this);
            onMove = null;
        }
    }
    private void OnMerge()
    {
        if((Vector2)transform.position == destination)
        {
            GameController.Instance.MergedBlock.Remove(this);
            PoolParty.Instance.GetPool("Blocks Pool").GetBackToPool(gameObject);
            onMerge = null;
        }
    }
    public void MoveDown()
    {
        transform.position -= transform.up * gravityMultiplier * 9.82f * Time.deltaTime;
    }
    public void Push()
    {
        gravityMultiplier = fastGravityMultiplier;
    }
}