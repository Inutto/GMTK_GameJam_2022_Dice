using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace CustomGrid
{
    public enum ObjectType
    {
        Wall,
        Pit,
        Enemy,
        Player,
    }

    public class GridObject : MonoBehaviour, IComparable<GridObject>
    {
        public ObjectType Type;

        [Header("World Metrics")]
        // not sure if need this, just use unit grid
        [SerializeField] Vector2 gridToWorldScale;
        [SerializeField] Vector2 gridToWorldOffset;


        [Header("Movement")]
        [SerializeField, Range(1, 10)] protected float rotateSpeed = 5f;
        [SerializeField, Range(0.1f, 2)] float moveDuration = 0.2f;

        [SerializeField] bool canMove = true;

        [Header("Health")]
        [SerializeField] protected int health;

        


        public Vector2Int GridPosition { get; protected set; }

        protected bool _isMoving;
        protected Tweener _moveTweener;
        protected Tweener _rotateTweener;
        
        private void Awake()
        {
            GridPosition = WorldToGridPos();
            
            
        }

        protected virtual void Start()
        {
            EventManager.Instance.NextActor.AddListener(OnNextActor);
            EventManager.Instance.InflictDamage.AddListener(OnTakeDamage);
        }


        private void FixedUpdate()
        {

        }

        protected virtual void OnDisable()
        {
            EventManager.Instance.NextActor.RemoveListener(OnNextActor);
            EventManager.Instance.InflictDamage.RemoveListener(OnTakeDamage);
        }

        #region Encapsuled Actions

        protected void MoveAndRollOne(Vector2Int delta, bool callFinish)
        {
            GridPosition += delta;

            if(delta.x != 0)
            {
                var anchor = transform.position + new Vector3(delta.x > 0 ? 0.5f : -0.5f, 0, 0.5f);
                var axis = Vector3.Cross(Vector3.back, delta.x > 0 ? Vector3.right : Vector3.left);
                StartCoroutine(Roll(anchor, axis, callFinish));
            }
            else if(delta.y != 0)
            {
                var anchor = transform.position + new Vector3(0, delta.y > 0 ? 0.5f : -0.5f, 0.5f);
                var axis = Vector3.Cross(Vector3.back, delta.y > 0 ? Vector3.up : Vector3.down);
                StartCoroutine(Roll(anchor, axis, callFinish));
            }
        }

        protected void KnockBack(Vector2Int delta)
        {
            GridPosition += delta;

            Move(delta, false);
        }

        protected void DoNothingEndTurn()
        {
            // Grid Pos stays the same

            EndTurn();
        }

        #endregion

        #region EventListeners

        /// <summary>
        /// If obj is me then its my turn, do some shit.
        /// </summary>
        /// <param name="obj">Accepted object from event</param>
        protected virtual void OnNextActor(GridObject obj)
        {
            if (obj != this)
            {
                return;
            } 
            else
            {
                DebugF.Log(obj.gameObject.name, obj.gameObject);
            }
        }

        /// <summary>
        /// called when InflictDamage event is invoked
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="damage"></param>
        protected virtual void OnTakeDamage(GridObject source, GridObject target, int damage)
        {
            if (target != this) return;
        }

        #endregion

        #region BasicActions

        IEnumerator Roll(Vector3 anchor, Vector3 axis, bool callFinish)
        {
            _isMoving = true;

            for(int i = 0; i < (90f / rotateSpeed); i++)
            {
                transform.RotateAround(anchor, axis, rotateSpeed);
                yield return new WaitForSeconds(0.01f);
            }

            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            _isMoving = false;

            // TEST: rotate dice
            DiceAttributeBehavior diceAttribute;
            if (TryGetComponent<DiceAttributeBehavior>(out diceAttribute))
            {
                diceAttribute.UpdateNum(axis);
            }

            // Combat calculation

            if(callFinish) EventManager.Instance.CallFinishAction(this);
            DebugF.Log("Finish Roll", this.gameObject);
        }

        void Move(Vector2Int delta, bool callFinish)
        {
            _isMoving = true;

            transform.DOMove(
                new Vector3(
                        transform.position.x + delta.x,
                        transform.position.y + delta.y,
                        0
                    ),
                moveDuration).OnComplete
                (() =>
                    {
                        _isMoving = false;
                        if(callFinish) EventManager.Instance.CallFinishAction(this);
                        DebugF.Log("Finish Move", this.gameObject);
                    }
                );
        }

        void EndTurn()
        {
            DebugF.Log("End Turn");
            
            StartCoroutine(WaitForRollTime());
        }

        IEnumerator WaitForRollTime() {
            
         
            yield return new WaitForSeconds((90f / rotateSpeed * 0.01f));
            EventManager.Instance.CallFinishAction(this);

        }

        #endregion

        #region util
        // not sure if need this
        protected Vector2 GridToWorldPos()
        {
            Vector2 pos = transform.position;
            pos *= gridToWorldScale;
            pos += gridToWorldOffset;
            return pos;
        }

        protected Vector2Int WorldToGridPos()
        {
            Vector2 pos = transform.position;
            pos -= gridToWorldOffset;
            pos /= gridToWorldScale;
            return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        }

        protected virtual bool TryMoveOnGrid(Vector2Int delta)
        {
            if (!canMove) return false;

            var msg = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var obj);
            if (msg != TryGetObjMsg.FLOOR) return false;

            // NOTICE: nolonger move GridPosition in this method.
            // It's moved to Encapsulated Actions.
            //GridPosition += delta;
            return true;
        }

        #endregion

        #region health

        protected virtual void UpdateHealth(int delta)
        {
            // just used to clamp health I guess, behavior in OnTakeDamage or OnHeal

            health += delta;
            if(health <= 0)
            {
                health = 0;
            }
            else if(health > 6)
            {
                health = 6;
            }
        }

        protected virtual void SetHealth(int value)
        {
            health = value;
            Mathf.Clamp(health, 0, 6);
        }

        public int GetHealth()
        {
            return health;
        }

        #endregion health

        public int CompareTo(GridObject other)
        {
            return this.gameObject.name.CompareTo(other.gameObject.name);
        }
    }
}


