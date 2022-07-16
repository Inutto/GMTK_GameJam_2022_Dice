using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomGrid;

public class EnemyBehavior : GridObject
{
    protected override void OnNextActor(GridObject obj)
    {
        if (obj != this) return;

        // random move for test
        Vector2Int up = new Vector2Int(0, 1);
        Vector2Int down = new Vector2Int(0, -1);
        Vector2Int left = new Vector2Int(-1, 0);
        Vector2Int right = new Vector2Int(1, 0);

        if (TryMoveOnGrid(up))
        {
            MoveAndRollOne(up, true);
        }
        else if (TryMoveOnGrid(down))
        {
            MoveAndRollOne(down, true);
        }
        else if (TryMoveOnGrid(left))
        {
            MoveAndRollOne(left, true);
        }
        else if (TryMoveOnGrid(right))
        {
            MoveAndRollOne(right, true);
        }
        else
        {
            DebugF.LogWarning("No Where To Go");
        }
    }

    protected override void OnTakeDamage(GridObject source, GridObject target, int damage)
    {
        if (target != this) return;

        UpdateHealth(-damage);
        if (health == 0)
        {
            EventManager.Instance.CallEnemyDied(this);
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
                    EventManager.Instance.CallEnemyDied(this);
                    // TODO: change this later
                    gameObject.SetActive(false);
                }
                else if (msgBehind == TryGetObjMsg.OUTOFBOUNDS)
                {
                    EventManager.Instance.CallEnemyDied(this);
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
