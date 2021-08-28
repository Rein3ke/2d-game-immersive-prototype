using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _currentGameController;
    public static GameController CurrentGameController { get => _currentGameController; }

    private SceneController sceneController;
    private CursorController cursorController;

    [SerializeField]
    private GameObject playerUIPrefab;
    [SerializeField]
    private float playerHealth = 100.0f;
    private bool isGameOver = false;

    public float PlayerHealth
    {
        get => playerHealth;
        set
        {
            playerHealth = Mathf.Clamp(value, 0.0f, 100.0f);
            PlayerHealthChange();
        }
    }


    private void Awake()
    {
        if (_currentGameController != null && _currentGameController != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _currentGameController = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneController = GetComponent<SceneController>();
        cursorController = GetComponent<CursorController>();

        if (!sceneController.CurrentScene.name.Equals("Menu")) { Instantiate(playerUIPrefab); }
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space) && !isGameOver)
        {
            PlayerHealth -= 10;
        }

        if (Input.GetKey(KeyCode.N) && !isGameOver)
        {
            LoadingNextScene();
        }

        if (PlayerHealth <= 0.0f && !isGameOver)
        {
            PlayerDeath();
            isGameOver = true;
        }
    }

    public void loadNextScene()
    {
        LoadingNextScene();
    }

    public void loadMenu()
    {
        LoadingMainMenuScene();
    }

    // Events

    public event Action onPlayerHealthChange;
    public void PlayerHealthChange()
    {
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange();
        }
    }

    public event Action onPlayerDeath;
    public void PlayerDeath()
    {
        if (onPlayerDeath != null)
        {
            Debug.Log("Event: Player death!");
            onPlayerDeath();
        }
    }

    public event Action onLoadingNextScene;
    public void LoadingNextScene()
    {
        if (onLoadingNextScene != null)
        {
            Debug.Log("Event: Loading next scene!");
            onLoadingNextScene();
        }
    }

    public event Action onLoadingMainMenuScene;
    public void LoadingMainMenuScene()
    {
        if (onLoadingMainMenuScene != null)
        {
            Debug.Log("Event: Loading main menu scene!");
            onLoadingMainMenuScene();
        }
    }
}
