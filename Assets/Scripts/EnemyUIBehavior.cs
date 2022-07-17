using CustomGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUIBehavior : MonoBehaviour
{

    public int health;
    public GameObject enemyObj;
    public Vector3 followOffset; // test out an appropricate one
    [SerializeField] float lerpSpeed = 4f;

    [Header("UI Refernece")]
    public TMP_Text num;
    public Image fill;
    


    

    private void Start()
    {

        EventManager.Instance.EnemyDied.AddListener(OnEnemyDied);

    }

    
    private void OnDisable()
    {

        EventManager.Instance.EnemyDied.RemoveListener(OnEnemyDied);

    }


    private void OnEnemyDied(GridObject obj, bool isSquashed, int dmg)
    {
        DebugF.Log("Enemy Behavior: On this dead: " + obj);
        if(obj.gameObject == enemyObj) Destroy(gameObject);
    }

    void Update()
    {
        // Follow
        var stepPosition = Vector3.Lerp(transform.position, enemyObj.transform.position + followOffset, Time.deltaTime * lerpSpeed);
        transform.position = stepPosition;

        // Update UI based on Health
        num.text = health.ToString();
        var ratio = health / 6f;
        fill.fillAmount = ratio;

    }
}
