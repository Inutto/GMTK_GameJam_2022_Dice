using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomGrid;

public class EnemyBehavior : GridObject
{
    protected override void OnNextActor(GridObject obj)
    {
        if (obj != this) return;

        // Use pathfinder
        var delta = GridManager.Instance.GenerateNextMove(this, GameStateManager.Instance.player);
        var msg = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var temp);
        if(msg == TryGetObjMsg.FLOOR)
        {
            MoveAndRollOne(delta, true);
        } else 
        {
            DebugF.LogWarning("Pathfind failed: not valid next move :" +
               delta.ToString().ToRichTextColor(Color.red) + " for GridObject: " +
               this.ToString().ToRichTextColor(Color.green));


            DoNothingEndTurn();
            return;

        }


        

    }

    protected override void OnTakeDamage(GridObject source, GridObject target, int damage)
    {
        if (target != this) return;
        int overDamage = damage - health;

        UpdateHealth(-damage);
        if (health == 0)
        {
            Vector2Int delta = GridPosition - source.GridPosition;
            EventManager.Instance.CallEnemyDied(this, delta.magnitude == 1, damage - overDamage);
            // TODO: change this later
            gameObject.SetActive(false);
        }
        else
        {
            // since InflictDamage is invoked before player move, delta length should be 1 when player
            // crashes enemy, but longer when player is area attacking
            Vector2Int delta = GridPosition - source.GridPosition;
            var msgBehind = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var obj);
            if(delta.magnitude == 1)    // only knockback when player crash enemy
            {
                if (msgBehind == TryGetObjMsg.FLOOR)
                {
                    KnockBack(delta);
                }
                else if (msgBehind == TryGetObjMsg.SUCCESS)
                {
                    if(obj.Type == ObjectType.Enemy)
                    {
                        EventManager.Instance.CallInflictDamage(this, obj, 1);
                    }
                    EventManager.Instance.CallEnemyDied(this, true, health);
                    // TODO: change this later
                    gameObject.SetActive(false);
                }
                else if (msgBehind == TryGetObjMsg.OUTOFBOUNDS)
                {
                    EventManager.Instance.CallEnemyDied(this, true, health);
                    // TODO: change this later
                    gameObject.SetActive(false);
                }
            }
        }
    }

    void Die()
    {

    }

}
