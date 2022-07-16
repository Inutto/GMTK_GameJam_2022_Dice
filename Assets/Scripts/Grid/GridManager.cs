using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomGrid
{
    public enum TryGetObjMsg
    {
        FLOOR,
        OUTOFBOUNDS,
        SUCCESS,
    }

    public class GridManager : MonoSingleton<GridManager>
    {
        Dictionary<Vector2Int, GridObject> GridData = new();
        [SerializeField] Vector2Int dimensions;

        private void Start()
        {
            var objects = FindObjectsOfType<GridObject>();
            foreach (var obj in objects)
            {
                if (obj.GridPosition.x > dimensions.x || obj.GridPosition.y > dimensions.y)
                {
                    Debug.LogError("GridObject: " + obj + "out of bounds.");
                    continue;
                }
                GridData.Add(obj.GridPosition, obj);
            }

            EventManager.Instance.FinishAction.AddListener(OnActionFinish);
        }

        //public GridManager(Vector2Int dimensions)
        //{
        //    this.dimensions = dimensions;

        //    var objects = FindObjectsOfType<GridObject>();
        //    foreach (var obj in objects)
        //    {
        //        if(obj.GridPosition.x > dimensions.x || obj.GridPosition.y > dimensions.y)
        //        {
        //            Debug.LogError("GridObject: " + obj + "out of bounds.");
        //            continue;
        //        }
        //        GridData.Add(obj.GridPosition, obj);
        //    }
        //}

        public bool AddObject(GridObject obj)
        {
            return GridData.TryAdd(obj.GridPosition, obj);
        }

        public bool RemoveObject(GridObject obj)
        {
            if (GridData.ContainsKey(obj.GridPosition))
            {
                GridData.Remove(obj.GridPosition);
                return true;
            }

            return false;
        }

        public void OnActionFinish(GridObject obj)
        {
            UpdateGrid();
        }

        /// <summary>
        /// Try to get grid obj at pos. Note: obj is empty both when out of bounds or there's only floor.
        /// Refer to TryGetObjMsg to see what is hit.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="obj"></param>
        /// <returns>TryGetObjMsg: SUCCESS, OUTOFBOUNDS, FLOOR</returns>
        public TryGetObjMsg TryGetObjectAt(Vector2Int pos, out GridObject obj)
        {
            if(pos.x > dimensions.x || pos.y > dimensions.y || pos.x < 0 || pos.y < 0)
            {
                obj = null;
                return TryGetObjMsg.OUTOFBOUNDS;
            }

            if (GridData.ContainsKey(pos))
            {
                obj = GridData[pos];
                return TryGetObjMsg.SUCCESS;
            }

            obj = null;
            return TryGetObjMsg.FLOOR;
        }

        public void UpdateGrid()
        {
            var objects = GridData.Values.ToList();
            GridData.Clear();
            foreach(var obj in objects)
            {
                GridData.Add(obj.GridPosition, obj);
            }
        }
    }
}

