using CustomGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoSingleton<LevelManager>
{


    [SerializeField] int maxBuildIndex;

    private void Start()
    {
        EventManager.Instance.PlayerDied.AddListener(OnPlayerDead);

    }


    private void OnDisable()
    {
        EventManager.Instance.PlayerDied.RemoveListener(OnPlayerDead);

    }


    private void OnPlayerDead(GridObject obj)
    {
        // SetActive the restart panel

        DebugF.Log("Display Player Dead");


    }


    public void LoadNextLevelAfterSeconds(float time)
    {
        TimerF.DoThisAfterSeconds(time, LoadNextLevel);
    }



    public void LoadNextLevel()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex;
        var nextIndex = Mathf.Min(maxBuildIndex, buildIndex + 1);

        SceneManager.LoadScene(nextIndex);
    }

    public void RestartLevel()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(buildIndex);
    }




}
