using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGrid
{
    public class GameStateManager : MonoSingleton<GameStateManager>
    {
        List<GridObject> actors;

        int currentIndex;

        // Start is called before the first frame update
        void Start()
        {
            EventManager.Instance.FinishAction.AddListener(OnFinishAction);


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
            if (actors[currentIndex] != obj)
            {
                Debug.LogError("Grid Object: " + obj + "at index: " + actors.IndexOf(obj) + " finished, but is out of order. Current index is: " + currentIndex);
                return;
            }
            
            // if depleted all grid objects in this turn
            if (currentIndex == actors.Count)
            {
                currentIndex = 0;
                EventManager.Instance.CallTurnFinished();
            }
            else
            {
                currentIndex += 1;
                EventManager.Instance.CallNextActor(actors[currentIndex]);
            }
        }

        
    }
}


