using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using CustomGrid;

[RequireComponent(typeof(DiceAttributeBehavior))]
[RequireComponent(typeof(DamageAreaBehavior))]
public class PlayerBehavior : GridObject
{
    bool isMyTurn;

    [SerializeField] DiceAttributeBehavior _dice;
    [SerializeField] DamageAreaBehavior _damageAreaBehavior;

    [Header("Damage Area")]
    [SerializeField] List<Vector2Int> currentAreaList;
    bool canDrawGizmos = false;

    private void Update()
    {
        if (!isMyTurn) return;

        if (!_isMoving)
        {
            var hor = Input.GetAxis("Horizontal");
            var ver = Input.GetAxis("Vertical");
            Vector2Int delta = Vector2Int.zero;
            if (hor != 0)
            {
                //delta = new Vector2Int(hor > 0 ? 1 : -1, 0);
                delta = hor > 0? Vector2Int.right : Vector2Int.left;
            }
            else if (ver != 0)
            {
                //delta = new Vector2Int(0, ver > 0 ? 1 : -1);
                delta = ver > 0 ? Vector2Int.up : Vector2Int.down;
            }

            // if no input return
            if (delta == Vector2Int.zero) return;

            var msg = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var obj);
            switch (msg)
            {
                case TryGetObjMsg.FLOOR:
                    PlayerMove(delta);
                    isMyTurn = false;
                    break;
                case TryGetObjMsg.OUTOFBOUNDS:
                    break;
                case TryGetObjMsg.SUCCESS:
                    switch (obj.Type)
                    {
                        case ObjectType.Wall:
                            break;
                        case ObjectType.Pit:
                            break;
                        case ObjectType.Enemy:
                            AttackTarget(obj, delta);
                            PlayerMove(delta);
                            isMyTurn = false;
                            break;
                        case ObjectType.Player:
                            DebugF.LogError("WTF? There's 2 players or delta is falsly calculated.");
                            break;
                        default:
                            
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    void PlayerMove(Vector2Int delta)
    {
        MoveAndRollOne(delta, false);
        DebugF.Log("Ready to Damage");
        DamageArea(delta);
    }

    void AttackTarget(GridObject obj, Vector2Int delta)
    {
        EventManager.Instance.CallInflictDamage(this, obj, _dice.GetFaceNum(DeltaToDirString(delta)));

    }

    void DamageArea(Vector2Int delta)
    {
        // TEST: damange area
        DamageAreaBehavior damageAreaBehavior;
        if (TryGetComponent<DamageAreaBehavior>(out damageAreaBehavior))
        {
            DebugF.Log("Apply Damage Area");
            currentAreaList = damageAreaBehavior.GetDamageArea(_dice.GetFaceNum(DeltaToDirString(delta))-1, delta);
            canDrawGizmos = true;
            StartCoroutine(TimerF.DoThisAfterSeconds(2f, () => { canDrawGizmos = false; }));
        }

        // add 0.1f in time just in case
        StartCoroutine(TimerF.DoThisAfterSeconds(90f / rotateSpeed * 0.01f + 0.1f, () => 
            {
                // since we've already rolled, just get dmg from the face facing forward (down the board)
                DamageAllEnemyInArea(currentAreaList, _dice.GetFaceNum("forward"), true);
            }));
    }

    void DamageAllEnemyInArea(List<Vector2Int> area, int dmg, bool callFinish)
    {
        var enemies = FindAllEnemyInArea(area, Vector2Int.zero);  // delta is zero here bcz I thought I'll call this before moving but apparently now its called afterwards
        foreach (var enemy in enemies)
        {
            EventManager.Instance.CallInflictDamage(this, enemy, dmg);
            DebugF.Log("Damaging enemy " + enemy.ToString() + " for damage of " + dmg);
        }

        if(callFinish) EventManager.Instance.CallFinishAction(this);
    }

    List<GridObject> FindAllEnemyInArea(List<Vector2Int> currentArea, Vector2Int delta)
    {
        List<GridObject> ans = new();
        GridObject obj;
        foreach(var item in currentArea)
        {
            var msg = GridManager.Instance.TryGetObjectAt(GridPosition + delta + item, out obj);
            if(msg == TryGetObjMsg.SUCCESS)
            {
                if(obj.Type == ObjectType.Enemy)
                {
                    ans.Add(obj);
                }
            }
        }
        return ans;
    }

    protected override void OnNextActor(GridObject obj)
    {
        if (obj != this) return;

        isMyTurn = true;
    }

    string DeltaToDirString(Vector2Int delta)
    {
        if (delta == Vector2Int.right) return "right";
        if (delta == Vector2Int.left) return "left";
        if (delta == Vector2Int.up) return "up";
        if (delta == Vector2Int.down) return "down";

        // I'm not sure what should default be so here's a forward for you :(
        DebugF.LogError("You gave me the wrong Delta mate. Returning default 'forward'.");
        return "forward";
    }


    private void OnDrawGizmos()
    {
        if(!canDrawGizmos) return;

        // TEmp
        Gizmos.color = Color.red; 
        foreach(var point in currentAreaList)
        {
            var drawPos = transform.position + new Vector3(point.x, point.y, 0);
            Gizmos.DrawSphere(drawPos, 0.4f);
        }
        
    }
}
