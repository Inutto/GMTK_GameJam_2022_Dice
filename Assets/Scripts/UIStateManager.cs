using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CustomGrid;

public class UIStateManager : MonoSingleton<UIStateManager>
{

    // Manually set them.
    public TMP_Text healthText;
    public Image healthFillImage;


    private void Update()
    {
        // Hard ref, since we do have one
        var player = GameStateManager.Instance.player;
        if (player != null)
        {

            // hard get health and update
            var currentHealth = player.GetHealth();
            var ratio = currentHealth / 6.0f;

            healthText.text =  currentHealth.ToString();
            healthFillImage.fillAmount = ratio;




        } else
        {
            DebugF.LogError("Player should not be null at GSM, Shutting down self for notice");
            gameObject.SetActive(false);
            return;
        }
    }







}
