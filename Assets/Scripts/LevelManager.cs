using CustomGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{


    [SerializeField] int maxBuildIndex;
    public GameObject GameLoseMenu;
    public GameObject GameWinMenu;

    private void Start()
    {
        EventManager.Instance.PlayerDied.AddListener(OnPlayerDead);
        EventManager.Instance.LevelClear.AddListener(OnLevelClear);

    }

 

    private void OnDisable()
    {
        EventManager.Instance.PlayerDied.RemoveListener(OnPlayerDead);
        EventManager.Instance.LevelClear.RemoveListener(OnLevelClear);

    }



    private void OnLevelClear()
    {

        DebugF.Log("Display Win");

        // TEST: we should test this
        GameWinMenu.SetActive(true);
    }

    private void OnPlayerDead(GridObject obj)
    {
        // SetActive the restart panel

        DebugF.Log("Display Player Dead");

        // TEST: we should test this
        GameLoseMenu.SetActive(true);

        

    }


    public void LoadNextLevelAfterSeconds(float time)
    {
        StartCoroutine(TimerF.DoThisAfterSeconds(time, LoadNextLevel));
    }



    public void LoadNextLevel()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex;
        var nextIndex = Mathf.Min(maxBuildIndex, buildIndex + 1);

        SceneManager.LoadScene(nextIndex);
    }

    public void RestartLevel()
    {
        //var buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }




}
