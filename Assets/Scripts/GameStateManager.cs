using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGrid
{
    public class GameStateManager : MonoSingleton<GameStateManager>
    {
        public float turnWaitTime;

        public GameObject ParticlePrefab;
        public GameObject DeathParticlePrefab;
        public float USELESS_DOES_NOTHING;

        // Remember to add all actor types here (exclude player)
        readonly List<ObjectType> actorTypes = new()
        {
            ObjectType.Enemy,
        };

        public List<GridObject> actors = new();
        public GridObject player;

        int currentIndex;

        // Start is called before the first frame update
        void Start()
        {
            EventManager.Instance.FinishAction.AddListener(OnFinishAction);
            EventManager.Instance.EnemyDied.AddListener(OnEnemyDied);

            var gridObjs = FindObjectsOfType<GridObject>();
            foreach(GridObject obj in gridObjs)
            {
                if (actorTypes.Contains(obj.Type))
                {
                    actors.Add(obj);
                }

                if (obj.Type == ObjectType.Player)
                {
                    player = obj;
                }
            }

            // TODO: Remove Later
            ResetIndexAndCallPlayer();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate()
        {
            //Debug.Log(currentIndex);
        }

        private void OnDisable()
        {
            EventManager.Instance.FinishAction.RemoveListener(OnFinishAction);
            EventManager.Instance.EnemyDied.RemoveListener(OnEnemyDied);
        }

        public void AddActor(GridObject obj)
        {
            actors.Add(obj);
        }
        public void RemoveActor(GridObject obj)
        {
            int index = actors.IndexOf(obj);
            actors.Remove(obj);
            if(index < currentIndex)
                currentIndex--;
        }

        public void OnFinishAction(GridObject obj)
        {
            DebugF.Log(obj.ToString());

            StartCoroutine(TimerF.DoThisAfterSeconds(turnWaitTime, () =>
            {
                // if player
                if (currentIndex == -1)
                {
                    EventManager.Instance.CallNextActor(player);
                    currentIndex++;
                    return;
                }

                //if (actors[currentIndex] != obj)
                //{
                //    Debug.LogError("Grid Object: " + obj + "at index: " + actors.IndexOf(obj) + " finished, but is out of order. Current index is: " + currentIndex);
                //    return;
                //}

                // if depleted all grid objects in this turn
                if (currentIndex == actors.Count)
                {
                    ResetIndexAndCallPlayer();
                    //EventManager.Instance.CallTurnFinished();
                }
                else
                {
                    EventManager.Instance.CallNextActor(actors[currentIndex]);
                    currentIndex++;
                }

            }));


        }

        public void OnEnemyDied(GridObject obj, bool isSquashed, int dmg)
        {
            DeathPuff(obj.transform.position);
            RemoveActor(obj);
            if (actors.Count == 0)
                EventManager.Instance.CallLevelClear();
        }

        public void Puff(Vector3 pos, List<Vector2Int> currentAreaList)
        {
            GameObject par;
            foreach (var area in currentAreaList)
            {
                par = Instantiate(ParticlePrefab);
                par.transform.position = pos + new Vector3(area.x, area.y, 0);
                par.GetComponent<ParticleSelfDestruct>().life = USELESS_DOES_NOTHING;
                //StartCoroutine(TimerF.DoThisAfterSeconds(particleTime, () => { Destroy(par); }));
            }

            par = Instantiate(ParticlePrefab);
            par.transform.position = pos;
            par.GetComponent<ParticleSelfDestruct>().life = USELESS_DOES_NOTHING;
            //StartCoroutine(TimerF.DoThisAfterSeconds(particleTime, () => { Destroy(par); }));
        }

        public void DeathPuff(Vector3 pos)
        {
            var par = Instantiate(DeathParticlePrefab);
            par.transform.position = pos;
            par.GetComponent<ParticleSelfDestruct>().life = USELESS_DOES_NOTHING;
        }

        #region temp
        void ResetIndexAndCallPlayer()
        {
            currentIndex = 0;
            EventManager.Instance.CallNextActor(player);
        }

        #endregion temp


    }
}


