using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private int sceneIndex;

    public Scene CurrentScene { get => SceneManager.GetActiveScene(); }

    private void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        GameController.Instance.onLoadingMainMenuScene += loadMenu;

        // Event Subscription
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void loadNextScene()
    {
        Debug.Log("Loading next scene...");
        if (sceneIndex == (SceneManager.sceneCountInBuildSettings - 1))
        {
            loadMenu();
        } else
        {
            SceneManager.LoadScene(++sceneIndex);
        }
    }

    private void loadMenu()
    {
        Debug.Log("Loading Main Menu...");
        SceneManager.LoadScene(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Current scene: " + scene.name);
    }

    private void OnDisable()
    {
        GameController.Instance.onLoadingMainMenuScene -= loadMenu;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
