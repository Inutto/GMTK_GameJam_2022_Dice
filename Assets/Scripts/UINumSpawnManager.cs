using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using TMPro;

using CustomGrid;

public class UINumSpawnManager : MonoSingleton<UINumSpawnManager>
{


    [SerializeField] float textHeightInit;
    [SerializeField] float textHeightDelta;
    [SerializeField] float horizontalRandomRange = 2f;
    [SerializeField] GameObject spawnedText;

    private void Start()
    {
        EventManager.Instance.InflictDamage.AddListener(OnInflictDamage);
        EventManager.Instance.NextActor.AddListener(OnNextActor);
    }

    private void OnNextActor(GridObject obj)
    {
        if(obj.Type == ObjectType.Player)
        {
            // Hard ref, because its easier
            var playerPos = GameStateManager.Instance.player.gameObject.transform.position;
            var drawPos = playerPos + new Vector3(0, 0, -2);
            DrawTextAtPosition(drawPos, "Your Turn!", Color.red, 0, Ease.OutSine);
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.InflictDamage.RemoveListener(OnInflictDamage);
        EventManager.Instance.NextActor.RemoveListener(OnNextActor);
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
            damageColor = Color.yellow;
        } else if(target.Type == ObjectType.Player)
        {
            damageColor = Color.red;
        }

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


        textComponent.text = content;
        textComponent.color = color; 
        DebugF.Log("Spawned Text Content: " + content);


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
