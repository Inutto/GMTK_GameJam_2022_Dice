using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{

    public AudioSource OnMove; // roll
    public AudioSource OnAttack; // ground effect
    public AudioSource OnSquashed; // expo

    public AudioSource OnEnemyDied;


    
    public AudioSource OnGameWin; // game win
    public AudioSource OnGameLose;


    private void Start()
    {
        // Manulaly Set them.
    }

    void LoadPlayerConfig(AudioSource audioSource)
    {

    }

    void LoadEnemyConfig(AudioSource audioSource)
    {

    }






}
