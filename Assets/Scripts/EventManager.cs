using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

using CustomGrid;

public class EventManager : MonoSingleton<EventManager>
{
    #region events
    [SerializeField] public UnityEvent<GridObject> FinishAction;     // one actor finished their turn
    [SerializeField] public UnityEvent<GridObject> NextActor;        // next actor's turn

    [SerializeField] public UnityEvent<GridObject, GridObject, int> InflictDamage;
    [SerializeField] public UnityEvent<GridObject, bool, int> EnemyDied;        // call this when enemy dies

    [SerializeField] public UnityEvent<GridObject> PlayerDied;

    [SerializeField] public UnityEvent LevelClear;
    #endregion




    #region callers
    public void CallFinishAction(GridObject obj)
    {
        DebugF.Log(obj.ToString());
        FinishAction?.Invoke(obj);
    }

    public void CallNextActor(GridObject obj)
    {
        DebugF.Log(obj.ToString());
        NextActor?.Invoke(obj);
    }

    public void CallInflictDamage(GridObject source, GridObject target, int damage)
    {
        DebugF.Log(source.ToString() + target.ToString() + damage);
        InflictDamage?.Invoke(source, target, damage);
    }

    public void CallEnemyDied(GridObject obj, bool isSquashed, int fatalDmg)
    {
        DebugF.Log(obj.ToString() + "died of " + (isSquashed? "meele":"ranged"));
        EnemyDied?.Invoke(obj, isSquashed, fatalDmg);
    }

    public void CallPlayerDied(GridObject killer)
    {
        DebugF.Log(killer.ToString());
        PlayerDied?.Invoke(killer);
    }

    public void CallLevelClear()
    {
        DebugF.Log("Level Clear");
        LevelClear?.Invoke();
    }



    #endregion
}
