using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CustomGrid
{
    public enum ObjectType
    {
        Wall,
        Pit,
        Enemy,
        Player,
    }

    public class GridObject : MonoBehaviour
    {
        public ObjectType Type;

        // not sure if need this, just use unit grid
        [SerializeField] Vector2 gridToWorldScale;
        [SerializeField] Vector2 gridToWorldOffset;

        [SerializeField] float speed;
        [SerializeField] float moveDuration;

        [SerializeField] bool canMove = true;

        public Vector2Int GridPosition { get; private set; }

        protected bool _isMoving;
        protected Tweener _moveTweener;
        protected Tweener _rotateTweener;
        
        private void Awake()
        {
            GridPosition = WorldToGridPos();
            
            EventManager.Instance.NextActor.AddListener(OnNextActor);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate()
        {

        }

        private void OnDisable()
        {
            EventManager.Instance.NextActor.RemoveListener(OnNextActor);
        }

        protected void MoveAndRollOne(Vector2Int delta)
        {
            if(delta.x != 0)
            {
                var anchor = transform.position + new Vector3(delta.x > 0 ? 0.5f : -0.5f, 0, 0.5f);
                var axis = Vector3.Cross(Vector3.back, delta.x > 0 ? Vector3.right : Vector3.left);
                StartCoroutine(Roll(anchor, axis));
            }
            else if(delta.y != 0)
            {
                var anchor = transform.position + new Vector3(0, delta.y > 0 ? 0.5f : -0.5f, 0.5f);
                var axis = Vector3.Cross(Vector3.back, delta.y > 0 ? Vector3.up : Vector3.down);
                StartCoroutine(Roll(anchor, axis));
            }
        }

        protected virtual bool TryMoveOnGrid(Vector2Int delta)
        {
            if (!canMove) return false;
            
            var msg = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var obj);
            if(msg != TryGetObjMsg.FLOOR) return false;

            GridPosition += delta;
            return true;
        }

        /// <summary>
        /// If obj is me then its my turn, do some shit.
        /// </summary>
        /// <param name="obj">Accepted object from event</param>
        protected virtual void OnNextActor(GridObject obj)
        {
            if (obj != this) return;
        }

        protected virtual void ActionFinish()
        {
            EventManager.Instance.CallFinishAction(this);
        }

        #region util
        // not sure if need this
        private Vector2 GridToWorldPos()
        {
            Vector2 pos = transform.position;
            pos *= gridToWorldScale;
            pos += gridToWorldOffset;
            return pos;
        }

        private Vector2Int WorldToGridPos()
        {
            Vector2 pos = transform.position;
            pos -= gridToWorldOffset;
            pos /= gridToWorldScale;
            return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        }

        IEnumerator Roll(Vector3 anchor, Vector3 axis)
        {
            _isMoving = true;

            for(int i = 0; i < (90f / speed); i++)
            {
                transform.RotateAround(anchor, axis, speed);
                yield return new WaitForSeconds(0.01f);
            }

            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

            _isMoving = false;

            ActionFinish();
        }

        #endregion
    }
}


