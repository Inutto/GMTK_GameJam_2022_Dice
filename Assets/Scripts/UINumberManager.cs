using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using TMPro;

using CustomGrid;

public class UINumberManager : MonoSingleton<UINumberManager>
{


    [SerializeField] float textHeightInit;
    [SerializeField] float textHeightDelta;
    [SerializeField] GameObject spawnedText;

    private void Start()
    {
        EventManager.Instance.InflictDamage.AddListener(OnInflictDamage);
    }

    private void OnDisable()
    {
        EventManager.Instance.InflictDamage.RemoveListener(OnInflictDamage);
    }



    public void OnInflictDamage(GridObject source, GridObject target, int damage)
    {
        // Print Out who take the damage in the screen

        var damagePos = target.gameObject.transform.position + 
            new Vector3(0, 0, textHeightInit);

        var damageNumText = damage.ToString();
        Color damageColor = Color.white;
        if(target.Type == ObjectType.Enemy)
        {
            damageColor = Color.yellow;
        } else if(target.Type == ObjectType.Player)
        {
            damageColor = Color.red;
        }

        DrawTextAtPosition(damagePos, damageNumText, damageColor);



    }

    public void DrawTextAtPosition(Vector3 worldPosition, string content, Color color)
    {
        var newText = Instantiate(spawnedText, worldPosition, transform.rotation, gameObject.transform);
        if(!newText.TryGetComponent<TMP_Text>(out var textComponent)) return;


        textComponent.text = content;
        textComponent.color = color; 
        DebugF.Log("Spawned Text Content: " + content);

        newText.transform.DOMove(
            new Vector3(
                newText.transform.position.x,
                newText.transform.position.y,
                textHeightInit + textHeightDelta), 2.0f)
            .SetEase(Ease.OutSine)
            .OnComplete(
                () =>
                {
                    // Fade Out THe Text
                }
             );


            
        
    }

}
