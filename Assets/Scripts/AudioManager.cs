using CustomGrid;
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


    public AudioSource BGM1;
    public AudioSource BGM2;

    public float dockBGMVolume;
    public float notDockBGMVolume;


    private void Start()
    {

        SetBGMDock(false);
        PlayBGM();

        EventManager.Instance.InflictDamage.AddListener(OnInflictDamageEvent);
        EventManager.Instance.EnemyDied.AddListener(OnEnemyDieEvent);
       
    }

    private void OnEnemyDieEvent(GridObject obj, bool isSquashed, int fatalDmg)
    {
        OnEnemyDie();
        if (isSquashed) OnSquash();
    }

    private void OnInflictDamageEvent(GridObject source, GridObject target, int damage)
    {
        if(target.Type == ObjectType.Enemy)
        {
            OnEnemyTakeDamage();
        } else if (target.Type == ObjectType.Player)
        {
            OnPlayerTakeDamage();
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.InflictDamage.RemoveListener(OnInflictDamageEvent);
        EventManager.Instance.EnemyDied.RemoveListener(OnEnemyDieEvent);
    }

    void LoadPlayerConfig(AudioSource audioSource)
    {
        audioSource.pitch = 1f;
    }

    void LoadEnemyConfig(AudioSource audioSource)
    {
        audioSource.pitch = Random.Range(0.7f, 1.2f);
        audioSource.volume = 0.7f;
    }


    AudioSource CopyAudioSource(AudioSource source)
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = source.clip;
        newSource.volume = source.volume;
        return newSource;
    }

    void DisableSourceAfterPlay(AudioSource source)
    {
        StartCoroutine(TimerF.DoThisAfterSeconds(source.clip.length + 1f, () => { Destroy(source); }));
    }

    void SetBGMDock(bool isDock)
    {
        if (isDock)
        {
            BGM1.volume = dockBGMVolume;
            BGM2.volume = dockBGMVolume;
        } else
        {
            BGM1.volume = notDockBGMVolume;
            BGM2.volume = notDockBGMVolume;
        }

       
    }

    void PlayBGM()
    {
        // Play BGM
        BGM1.Play();
        BGM2.Play();
    }

    public void OnPlayerMove()
    {
        LoadPlayerConfig(OnMove);
        OnMove.Play();
    }

    public void OnEnemyMove()
    {
        //LoadEnemyConfig(OnMove);
        //OnMove.Play();


        var enemySource = CopyAudioSource(OnMove);
        LoadEnemyConfig(enemySource);
        enemySource.Play();
        DisableSourceAfterPlay(enemySource);


    }

    public void OnPlayerTakeDamage()
    {
        LoadPlayerConfig(OnAttack);
        OnAttack.Play();
    }

    public void OnEnemyTakeDamage()
    {
        var enemySource = CopyAudioSource(OnAttack);
        LoadEnemyConfig(enemySource);
        enemySource.Play();
        DisableSourceAfterPlay(enemySource);
    }

    public void OnSquash()
    {
        OnSquashed.Play();
    }

    public void OnEnemyDie()
    {
        OnEnemyDied.Play();
    }

    public void OnWin()
    {
        SetBGMDock(true);
        OnGameWin.Play();
    }

    public void OnLose()
    {
        SetBGMDock(true);
        OnGameLose.Play();
    }

    







}
