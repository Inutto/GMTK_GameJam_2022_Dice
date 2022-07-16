using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using CustomGrid;

public class PlayerBehavior : GridObject
{
    bool isMyTurn;

    private void Update()
    {
        if (!isMyTurn) return;

        if (!_isMoving)
        {
            var hor = Input.GetAxis("Horizontal");
            var ver = Input.GetAxis("Vertical");
            if (hor != 0)
            {
                Vector2Int delta = new Vector2Int(hor > 0 ? 1 : -1, 0);
                if (TryMoveOnGrid(delta))
                {
                    MoveAndRollOne(delta);
                    isMyTurn = false;
                }

                // cant move there, do sth
            }
            else if (ver != 0)
            {
                Vector2Int delta = new Vector2Int(0, ver > 0 ? 1 : -1);
                if (TryMoveOnGrid(delta))
                {
                    MoveAndRollOne(delta);
                    isMyTurn = false;
                }

                // cant move there, do sth
            }
        }
    }

    private void FixedUpdate()
    {
        
    }


    protected override void OnNextActor(GridObject obj)
    {
        if (obj != this) return;

        isMyTurn = true;
    }
}
