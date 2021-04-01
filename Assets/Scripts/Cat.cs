using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.CustomComponents;

public class Cat : MoveObject
{
    [SerializeField]
    private Image icon;

    #region Properties
    public Sprite Icon { get => icon.sprite; set => icon.sprite = value; }
    #endregion
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Move(Vector2 destination)
    {
        base.Move(destination);
    }
    protected override void FinishMove()
    {
        base.FinishMove();
    }
}
