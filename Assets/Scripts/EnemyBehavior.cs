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

}
