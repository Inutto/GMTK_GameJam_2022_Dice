using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGrid
{
    public class GameStateManager : MonoSingleton<GameStateManager>
    {
        // Remember to add all actor types here (exclude player)
        readonly List<ObjectType> actorTypes = new()
        {
            ObjectType.Enemy,
        };

        List<GridObject> actors = new();
        GridObject player;

        int currentIndex;

        // Start is called before the first frame update
        void Start()
        {
            EventManager.Instance.FinishAction.AddListener(OnFinishAction);

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
            currentIndex = -1;
            EventManager.Instance.CallNextActor(player);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnDisable()
        {
            EventManager.Instance.FinishAction.RemoveListener(OnFinishAction);
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

        void OnFinishAction(GridObject obj)
        {
            if (currentIndex == -1)
            {
                EventManager.Instance.CallNextActor(player);
                currentIndex++;
            }

            //if (actors[currentIndex] != obj)
            //{
            //    Debug.LogError("Grid Object: " + obj + "at index: " + actors.IndexOf(obj) + " finished, but is out of order. Current index is: " + currentIndex);
            //    return;
            //}
            
            // if depleted all grid objects in this turn
            if (currentIndex == actors.Count)
            {
                currentIndex = -1;
                //EventManager.Instance.CallTurnFinished();
            }
            else
            {
                currentIndex++;
                EventManager.Instance.CallNextActor(actors[currentIndex]);
            }
        }

        
    }
}


