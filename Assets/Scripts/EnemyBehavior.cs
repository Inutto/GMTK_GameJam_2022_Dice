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
            MoveAndRollOne(up);
        }
        else if (TryMoveOnGrid(down))
        {
            MoveAndRollOne(down);
        }
        else if (TryMoveOnGrid(left))
        {
            MoveAndRollOne(left);
        }
        else if (TryMoveOnGrid(right))
        {
            MoveAndRollOne(right);
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
            // since InflictDamage is invoked before player move, delta length should be 1
            Vector2Int delta = GridPosition - source.GridPosition;
            var msg = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var obj);
            if (msg == TryGetObjMsg.FLOOR)
            {
                KnockBack(delta);
            }
            else if (msg == TryGetObjMsg.SUCCESS)
            {
                if(obj.Type == ObjectType.Enemy)
                {
                    EventManager.Instance.CallInflictDamage(this, obj, 1);
                }
                EventManager.Instance.CallEnemyDied(this);
                // TODO: change this later
                gameObject.SetActive(false);
            }
            else if (msg == TryGetObjMsg.OUTOFBOUNDS)
            {
                EventManager.Instance.CallEnemyDied(this);
                // TODO: change this later
                gameObject.SetActive(false);
            }
            
        }
    }

    void Die()
    {

    }

}
