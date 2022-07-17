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
        [SerializeField] Dictionary<Vector2Int, GridObject> GridData = new();
        [SerializeField] Vector2Int dimensions; // start from 0, so 9 * 9 is actually 10 * 10, leftCornor = (0, 0)

        [Header("Pathfinder")]
        GridGraph map;


        private void UpdatePathfindingGridMap()
        {
            // init map
            map = new GridGraph(dimensions.x + 1, dimensions.y + 1);

            // init obstacles by griddata
            List<Vector2> _walls = new List<Vector2>();
            List<Vector2> _forests = new List<Vector2>();
            foreach (var pairs in GridData)
            {
                if(pairs.Value.Type == ObjectType.Wall || 
                    pairs.Value.Type == ObjectType.Pit || 
                    pairs.Value.Type == ObjectType.Enemy)
          
                {
                    _walls.Add(new Vector2(pairs.Key.x, pairs.Key.y));
                }
            }

            map.Walls = _walls;
            map.Forests = _forests;

        }

        public Vector2Int GenerateNextMove(Vector2Int startPosition, Vector2Int goalPosition, out List<Node> pathOutput)
        {

            UpdatePathfindingGridMap();

            var path = AStar.Search(map,
               map.Grid[startPosition.x, startPosition.y],
               map.Grid[goalPosition.x, goalPosition.y]);

            pathOutput = path;

            if (path == null || path.Count < 1)
            {
                DebugF.LogError("Path Error. You idiot. I will return a up for subs");
                return Vector2Int.zero;
            }
            else if (path.Count == 1)
            {
                DebugF.LogWarning("Next path is target. I will not move in this case.");
                return Vector2Int.zero;
            }

            var nextMove = path[0];
            var delta = new Vector2Int(
                    (int)nextMove.Position.x - startPosition.x,
                    (int)nextMove.Position.y - startPosition.y);

            return delta;
        }

        /// <summary>
        /// Generate the next direction (delta) for the start obj if he wants to fuck the goal
        /// return zero as default, if something stupid happens
        /// </summary>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public Vector2Int GenerateNextMove(GridObject start, GridObject goal, out List<Node> pathOutput)
        {

            UpdatePathfindingGridMap();

            Vector2Int startPosition = start.GridPosition;
            Vector2Int goalPosition = goal.GridPosition;

            return GenerateNextMove(startPosition, goalPosition, out pathOutput);

        }



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
            EventManager.Instance.EnemyDied.AddListener(OnEnemyDied);
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

        public void OnEnemyDied(GridObject obj, bool isSquashed, int dmg)
        {
            RemoveObject(obj);
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

