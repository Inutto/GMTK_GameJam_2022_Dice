using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

using CustomGrid;

public class EventManager : MonoSingleton<EventManager>
{
    #region events
    public UnityEvent<GridObject> FinishAction;     // one actor finished their turn
    public UnityEvent<GridObject> NextActor;        // next actor's turn
    public UnityEvent TurnFinished;     // turn finished, no more actors to act

    #endregion




    #region callers
    public void CallFinishAction(GridObject obj)
    {
        FinishAction?.Invoke(obj);
    }

    public void CallNextActor(GridObject obj)
    {
        NextActor?.Invoke(obj);
    }

    public void CallTurnFinished()
    {
        TurnFinished?.Invoke();
    }



    #endregion
}
