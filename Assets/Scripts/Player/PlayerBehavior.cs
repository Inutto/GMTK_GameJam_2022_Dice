using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomGrid;

public class PlayerBehavior : GridObject
{
    bool isMyTurn;

    private void Update()
    {
        
    }

    

    protected override void OnNextActor(GridObject obj)
    {
        if (obj != this) return;

        isMyTurn = true;
    }
}
