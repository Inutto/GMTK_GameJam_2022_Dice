using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomGrid;

public class EnemyBehavior : GridObject
{

    [Header("Damage Area")]
    [SerializeField] List<Vector2Int> currentAreaList;
    bool canDrawGizmos = false;

    [Header("UI Update")]
    public GameObject enemyUIPrefab;
    public EnemyUIBehavior enemyUIBehavior;

    protected override void Start()
    {
        base.Start();

        // Spawn the UI under World Layout transform
        var newUIObject = Instantiate(enemyUIPrefab,
            transform.position,
            transform.rotation,
            WorldLayoutUISpawnManager.Instance.gameObject.transform);

        enemyUIBehavior = newUIObject.GetComponent<EnemyUIBehavior>();
        enemyUIBehavior.enemyObj = gameObject;
    }

    private void Update()
    {
        enemyUIBehavior.health = health;
    }




    protected override void OnNextActor(GridObject obj)
    {
        if (obj != this) return;

        // Use pathfinder
        var delta = GridManager.Instance.GenerateNextMove(this, GameStateManager.Instance.player);
        var msg = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var temp);
        if(msg == TryGetObjMsg.FLOOR)
        {
            MoveAndRollOne(delta, false);
            DamageArea(delta);
        } else 
        {
            DebugF.Log("No Valid Input for direction " + delta.ToString().ToRichTextColor(Color.red) +
                " , Stay Still as a result");

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
            EventManager.Instance.CallEnemyDied(this, delta.magnitude == 0, damage - overDamage);
            // TODO: change this later
            gameObject.SetActive(false);
        }
        else
        {
            // since InflictDamage is invoked AFTER player move, delta length should be 0 when player
            // crashes enemy, but longer when player is area attacking
            Vector2Int delta = GridPosition - source.GridPosition;
            var msgBehind = GridManager.Instance.TryGetObjectAt(GridPosition + delta, out var obj);
            if(delta.magnitude == 0)    // only knockback when player crash enemy
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


    void DamageArea(Vector2Int delta)
    {
        DamageAreaBehavior damageAreaBehavior;
        if (TryGetComponent<DamageAreaBehavior>(out damageAreaBehavior))
        {
            currentAreaList = damageAreaBehavior.GetDamageArea(health - 1, delta);
            canDrawGizmos = true;
            StartCoroutine(TimerF.DoThisAfterSeconds(2f, () => { canDrawGizmos = false; }));
        }

        // add 0.1f in time just in case
        StartCoroutine(TimerF.DoThisAfterSeconds(90f / rotateSpeed * 0.01f + 0.1f, () =>
        {
            // since we've already rolled, just get dmg from the face facing forward (down the board)
            DamagePlayerInArea(currentAreaList, this.health - 1, true);
        }));

    }



    void DamagePlayerInArea(List<Vector2Int> area, int dmg, bool callFinish)
    {
        GridObject target = TryGetPlayerInArea(area);
        if(target != null)
        {
            // Fuck this player
            EventManager.Instance.CallInflictDamage(this, target, dmg);
            DebugF.Log("Damaging player " + target.name + " with damage " +
                dmg.ToString().ToRichTextColor(Color.red));
        } else
        {
            DebugF.Log("No Player in Area");
        }

        if (callFinish) EventManager.Instance.CallFinishAction(this);
    }


    GridObject TryGetPlayerInArea(List<Vector2Int> area)
    {
        GridObject obj;
        foreach (var item in area)
        {
            var msg = GridManager.Instance.TryGetObjectAt(GridPosition + item, out obj);
            if (msg == TryGetObjMsg.SUCCESS)
            {
                if (obj.Type == ObjectType.Player)
                {
                    DebugF.Log("Find Player");
                    return obj;
                }
            }
        }

        DebugF.Log("Fail to find player for Object: "
            + this.ToString().ToRichTextColor(Color.green));
            
        return null;
    }


    void Die()
    {
        //Whatever you wanna do, keep this. I don't know why i need this because event not working
        Destroy(enemyUIBehavior.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!canDrawGizmos) return;

        // TEmp
        Gizmos.color = Color.green;
        foreach (var point in currentAreaList)
        {
            var drawPos = transform.position + new Vector3(point.x, point.y, 0);
            Gizmos.DrawSphere(drawPos, 0.4f);
        }

    }


}
