using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGrid
{
    public class GridObject : MonoBehaviour
    {
        // not sure if need this, just use unit grid
        [SerializeField] Vector2 gridToWorldScale;
        [SerializeField] Vector2 gridToWorldOffset;

        [SerializeField] float speed;

        [SerializeField] bool canMove = true;

        public Vector2Int GridPosition { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate()
        {
            MoveToGridPos();
        }

        // use in fixed update
        void MoveToGridPos()
        {
            if (!canMove) return;

            transform.position = Vector2.MoveTowards(
                                        transform.position, 
                                        GridToWorldPos(), 
                                        speed * Time.fixedDeltaTime
                                    );
        }

        /// <summary>
        /// If obj is me then its my turn, do some shit.
        /// </summary>
        /// <param name="obj">Accepted object from event</param>
        protected virtual void OnNextActor(GridObject obj)
        {
            if (obj != this) return;
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

        #endregion
    }
}


