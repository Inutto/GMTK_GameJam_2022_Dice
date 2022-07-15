using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGrid
{
    public class MyGrid : MonoBehaviour
    {
        public enum TryGetObjMsg
        {
            FLOOR,
            OUTOFBOUNDS,
            SUCCESS,
        }

        Dictionary<Vector2Int, GridObject> GridData = new();
        Vector2Int dimensions;

        public MyGrid(Vector2Int dimensions)
        {
            this.dimensions = dimensions;

            var objects = FindObjectsOfType<GridObject>();
            foreach (var obj in objects)
            {
                if(obj.GridPosition.x > dimensions.x || obj.GridPosition.y > dimensions.y)
                {
                    Debug.LogError("GridObject: " + obj + "out of bounds.");
                    continue;
                }
                GridData.Add(obj.GridPosition, obj);
            }
        }

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

        /// <summary>
        /// Try to get grid obj at pos. Note: obj is empty both when out of bounds or there's only floor.
        /// Refer to TryGetObjMsg to see what is hit.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="obj"></param>
        /// <returns>TryGetObjMsg: SUCCESS, OUTOFBOUNDS, FLOOR</returns>
        public TryGetObjMsg TryGetObject(Vector2Int pos, out GridObject obj)
        {
            if(pos.x > dimensions.x || pos.y > dimensions.y)
            {
                obj = null;
                return TryGetObjMsg.OUTOFBOUNDS;
            }

            if (GridData.TryGetValue(pos, out var tempObj))
            {
                obj = tempObj;
                return TryGetObjMsg.SUCCESS;
            }

            obj = null;
            return TryGetObjMsg.FLOOR;
        }
    }
}

