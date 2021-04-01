using System;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.CustomComponents
{
    public class MoveObject : MonoBehaviour
    {
        protected Vector2 destination = new Vector2(); //Thằng này, cũng v à test rồi =))

        public static float normalGravityMultiplier = 0.5f, slowGravityMultiplier = 0.1f, fastGravityMultiplier = 1.2f;
        public float gravityMultiplier = slowGravityMultiplier, gravity = 9.82f;
        protected delegate void OnMoveDelegate();
        protected event OnMoveDelegate onMove;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (GameManager.GameState == GameState.play)
            {
                if (onMove != null)
                {
                    onMove.Invoke();
                }
            }
        }
        protected virtual void FixedUpdate()
        {
            if (GameManager.GameState == GameState.play)
            {
                OnStartMove(new Action(DoMove));
            }
        }
        public virtual void Move(Vector2 destination)
        {
            this.destination = destination;
            onMove += OnMove;
        }
        private void OnMove()
        {
            if ((Vector2)transform.position == destination)
            {
                DoOnFinishMove();
                FinishMove();
            }
        }
        protected virtual void FinishMove()
        {
            onMove = null;
        }
        protected virtual void OnStartMove(Action action)
        {
            if (onMove != null)
            {
                action();
            }
        }
        protected virtual void DoMove()
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, gravityMultiplier * gravity * Time.deltaTime);
        }
        protected virtual void DoOnFinishMove()
        {
            Debug.Log($"{name} done move");
        }
    }
}