using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using TMPro;

using CustomGrid;

public class WorldLayoutUISpawnManager : MonoSingleton<WorldLayoutUISpawnManager>
{


    [SerializeField] float textHeightInit;
    [SerializeField] float textHeightDelta;
    [SerializeField] float horizontalRandomRange = 2f;
    [SerializeField] GameObject spawnedText;

    private void Start()
    {
        EventManager.Instance.InflictDamage.AddListener(OnInflictDamage);
        EventManager.Instance.NextActor.AddListener(OnNextActor);
        EventManager.Instance.EnemyDied.AddListener(OnEnemyDie);
    }



    private void OnDisable()
    {
        EventManager.Instance.InflictDamage.RemoveListener(OnInflictDamage);
        EventManager.Instance.NextActor.RemoveListener(OnNextActor);
        EventManager.Instance.EnemyDied.RemoveListener(OnEnemyDie);
    }

    private void OnNextActor(GridObject obj)
    {
        if (obj.Type == ObjectType.Player)
        {

            var targetPos = obj.gameObject.transform.position + new Vector3(0, -4, -2);
            // Hard ref, because its easier
            var message = "Your Turn!";
            var color = Color.red;
            DrawTextAtPosition(targetPos, message, color, 0, Ease.OutSine);
        }

    }

    private void OnEnemyDie(GridObject obj, bool isSquashed, int fatalDmg)
    {
        if (isSquashed)
        {
            var targetPos = obj.gameObject.transform.position + new Vector3(0, 1, -2);
            var message = "Squashed!";
            Color color = Color.blue;

            // check enemy AI state
            var enemyBehavior = obj.gameObject.GetComponent<EnemyBehavior>();
            if(enemyBehavior.enemyAIType == EnemyBehavior.EnemyAIType.IDLE)
            {
                color = Color.green;
            }
            DrawTextAtPosition(targetPos, message, color, 1.5f, Ease.OutBounce);
        }
        
    }


    public void OnInflictDamage(GridObject source, GridObject target, int damage)
    {
        // Print Out who take the damage in the screen

        var targetPos = target.gameObject.transform.position;

        var damagePos = new Vector3(targetPos.x, targetPos.y, textHeightInit);

        var damageNumText = damage.ToString();
        Color damageColor = Color.white;
        if(target.Type == ObjectType.Enemy)
        {

            // check enemy state
            var enemyBehavior = target.GetComponent<EnemyBehavior>();
            if(enemyBehavior.enemyAIType == EnemyBehavior.EnemyAIType.IDLE)
            {
                damageColor = Color.green;
            } else
            {
                damageColor = Color.blue;
            }


            
        } else if(target.Type == ObjectType.Player)
        {
            damageColor = Color.red;

            // For Player: Spawn special UI at special location
            //var specialPos = new Vector3(targetPos.x - 3, targetPos.y - 1.5f, textHeightInit);
            //DrawTextAtPosition(specialPos, damageNumText, damageColor, horizontalRandomRange, Ease.OutSine);

        }

        // For all damage: spawn at dmg location
        DrawTextAtPosition(damagePos, damageNumText, damageColor, horizontalRandomRange, Ease.OutBounce);



    }

    /// <summary>
    /// Draw text at given world position. randomAmout can control randomOffset.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="content"></param>
    /// <param name="color"></param>
    /// <param name="isRandom"></param>
    public void DrawTextAtPosition(Vector3 worldPosition, string content, Color color, float randomAmount, Ease easeType)
    {
        var newText = Instantiate(spawnedText, worldPosition, transform.rotation, gameObject.transform);
        if(!newText.TryGetComponent<TMP_Text>(out var textComponent)) return;


        // If has fader, Set Fade
        if (newText.TryGetComponent<FadeSelfUITextBehavior>(out var fadeComponent))
        {
            fadeComponent.StartFade();
        }

        textComponent.text = content;
        textComponent.color = color; 
        

        DebugF.Log("Spawned Text Content: " + content + "at Position:" + worldPosition);


        var randomOffsetX = Random.Range(-randomAmount, randomAmount);
        var randomOffsetY = Random.Range(-randomAmount, randomAmount);
     
        

        newText.transform.DOMove(
            new Vector3(
                newText.transform.position.x + randomOffsetX,
                newText.transform.position.y + randomOffsetY,
                textHeightInit + textHeightDelta), 2.0f)
            .SetEase(easeType)
            .OnComplete(
                () =>
                {
                    // Fade Out THe Text
                }
             );


            
        
    }

}
