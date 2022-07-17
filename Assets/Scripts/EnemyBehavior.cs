using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomGrid;

public class EnemyBehavior : GridObject
{

    public enum EnemyAIType
    {
        FOLLOW,
        SIEGE, 
        WONDER,
    }

    

    [Header("AI Type")]
    public EnemyAIType enemyAIType = EnemyAIType.FOLLOW;

    [Header("Damage Area")]
    [SerializeField] List<Vector2Int> currentAreaList;
    bool canDrawGizmos = false;

    [Header("UI Update")]
    public GameObject enemyUIPrefab;
    public EnemyUIBehavior enemyUIBehavior;

    [Header("Visual")]
    [SerializeField] float visualLastTime = 0.2f;
    [SerializeField] GameObject enemyVisualizerPrefab;
    List<GameObject> myVisualizers;

    /// <summary>
    /// Init for enemyUI stuff
    /// </summary>
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


    /// <summary>
    /// Get the next move direction of this enemy. 
    /// CAUTION: this might be (0, 0)
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetNextMoveDelta()
    {
        var player = GameStateManager.Instance.player;
        List<Node> path = new List<Node>();

        switch (enemyAIType)
        {

            // Directly Chase. If too close to player, run.
            case EnemyAIType.FOLLOW:
                var delta_follow =  GridManager.Instance.GenerateNextMove(this, player, out path);
                if(path.Count == 1)
                {
                    return TryToFlee(delta_follow);
                } else
                {
                    return delta_follow;
                }

            // Only find somewhere near player
            case EnemyAIType.SIEGE:
                var offSet = new Vector2Int(Random.Range(-1, 1), Random.Range(-1, 1));
                return GridManager.Instance.GenerateNextMove(this.GridPosition, player.GridPosition + offSet, out path);

            // If too close to player, run. 
            case EnemyAIType.WONDER:
                var delta_wonder = GridManager.Instance.GenerateNextMove(this, player, out path);
                if (path.Count <= 3)
                {
                    return TryToFlee(delta_wonder);
                } else
                {
                    // Still Follow
                    return delta_wonder;
                }
                
            default:
                DebugF.LogWarning("No Valid AIType Selected: return (0, 0) as default");
                return Vector2Int.zero;

        }

        // temp, but use this first
        
    }


    /// <summary>
    /// When player realize delta is a dangerous decision, find a way to run
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    Vector2Int TryToFlee(Vector2Int delta)
    {
        // Try opposite first: -delta
        var msg = GridManager.Instance.TryGetObjectAt(GridPosition - delta, out var temp);
        if (msg == TryGetObjMsg.FLOOR)
        {
            return -delta;
        }

        // If not, find all directions
        var directionArray = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var direction in directionArray)
        {
            if (delta == direction) continue;

            var msg_2 = GridManager.Instance.TryGetObjectAt(GridPosition + direction, out var temp_2);
            if (msg_2 == TryGetObjMsg.FLOOR)
            {
                return direction;
            }
        }

        // no where to go
        DebugF.LogWarning("No where to go because everywhere is stuck, return (0, 0)");
        return Vector2Int.zero;
    }


    protected override void OnNextActor(GridObject obj)
    {
        if (obj != this) return;

        // Use pathfinder
        var delta = GetNextMoveDelta();
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

    void VisualizeDamageArea(Vector2Int delta)
    {
        if(myVisualizers == null)
        {
            myVisualizers = new(6);
            for(int i = 0; i < 7; i++)
            {
                myVisualizers.Add(Instantiate(enemyVisualizerPrefab));
                myVisualizers[i].transform.position = new Vector3(0, 0, -2000);
            }
        }

        if (delta == Vector2Int.zero) return;

        for (int i = 0; i < currentAreaList.Count; i++){
            var msg = GridManager.Instance.TryGetObjectAt(GridPosition + currentAreaList[i], out var temp);
            //Debug.Log(msg.ToString() + "at" + GridPosition + currentAreaList[i]);
            bool haveStuffonBlock = (msg == TryGetObjMsg.SUCCESS);
            myVisualizers[i].transform.position = transform.position + new Vector3(currentAreaList[i].x + delta.x, currentAreaList[i].y + delta.y, haveStuffonBlock? -0.6f : 0f);
        }
    }

    void ClearDamageArea()
    {
        if (myVisualizers == null)
        {
            myVisualizers = new(6);
            for (int i = 0; i < 7; i++)
            {
                myVisualizers.Add(Instantiate(enemyVisualizerPrefab));
                myVisualizers[i].transform.position = new Vector3(0, 0, -2000);
            }
        }

        foreach(var item in myVisualizers)
        {
            item.transform.position = new Vector3(0, 0, -2000);
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
            Vector2Int playerDelta = new();
            bool wasPlayer = source.TryGetComponent<PlayerBehavior>(out var player);
            if (wasPlayer)
            {
                playerDelta = player.LastDelta;
            }

            var msgBehind = GridManager.Instance.TryGetObjectAt(GridPosition + (wasPlayer ? playerDelta : delta), out var obj);
            if(delta.magnitude == 0)    // only knockback when player crash enemy
            {
                if (msgBehind == TryGetObjMsg.FLOOR)
                {
                    KnockBack(wasPlayer ? playerDelta : delta);
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

        VisualizeDamageArea(delta);
        // add 0.1f in time just in case
        StartCoroutine(TimerF.DoThisAfterSeconds(90f / rotateSpeed * 0.01f + 0.1f, () =>
        {
            // since we've already rolled, just get dmg from the face facing forward (down the board)
            DamagePlayerInArea(currentAreaList, this.health, true); 
        }));

        StartCoroutine(TimerF.DoThisAfterSeconds(visualLastTime, () => { ClearDamageArea(); }));

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
