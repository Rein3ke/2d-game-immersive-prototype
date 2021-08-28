using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private SceneController sceneController;
    private CursorController cursorController;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneController = GetComponent<SceneController>();
        cursorController = GetComponent<CursorController>();
    }

    public void loadNextScene()
    {
        sceneController.loadScene();
    }
}
