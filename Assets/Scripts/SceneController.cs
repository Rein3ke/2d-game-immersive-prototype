using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private Scene[] scenes;
    private int sceneIndex;

    private void Start()
    {
        scenes = new Scene[SceneManager.sceneCountInBuildSettings];
        sceneIndex = 0;
    }

    internal void loadScene()
    {
        Debug.Log("Load Scene");
        SceneManager.LoadScene(++sceneIndex);
    }
}
