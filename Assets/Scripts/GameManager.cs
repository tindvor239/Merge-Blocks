using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameState gameState;

    public enum GameState {start, menu, play, pause, gameover}
    #region Properties
    public GameState State { get => gameState; }
    #endregion

    public delegate void OnUpdate(GameState gameState);
    public event OnUpdate onUpdate;


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
        if(onUpdate != null)
        {
            if (Grid.Instance.IsFull && gameState != GameState.gameover)
            {
                gameState = GameState.gameover;
            }
            onUpdate.Invoke(gameState);
            onUpdate = null;
        }
    }
    public GameObject CreateObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab);
        return newObject;
    }
}
