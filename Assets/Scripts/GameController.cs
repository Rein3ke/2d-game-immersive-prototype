using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController CurrentGameController { get => _currentGameController; }

    public bool IsGameOver
    {
        get => isGameOver;
    }
    public float CurrentPlayerHealth
    {
        get => currentPlayerHealth;
        set
        {
            // Handle Health & Game Over (Player Death)
            currentPlayerHealth = Mathf.Clamp(value, 0.0f, 100.0f);
            PlayerHealthChange();

            if (CurrentPlayerHealth <= 0.0f)
            {
                PlayerDeath();
                isGameOver = true;
            }
        }
    }

    public InputController InputController
    {
        get => _inputController;
    }

    private static GameController _currentGameController;

    private SceneController sceneController;
    private CursorController cursorController;
    private InputController _inputController;
    private float currentPlayerHealth   = 0.0f;
    private bool isGameOver = false;

    [SerializeField]
    private GameObject playerUIPrefab;
    [SerializeField]
    private float maxPlayerHealth       = 100.0f;

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
        sceneController     = GetComponent<SceneController>();
        cursorController    = GetComponent<CursorController>();
        _inputController     = GetComponent<InputController>();

        currentPlayerHealth = maxPlayerHealth;

        // Instantiate Player UI if current scene isn't "Main Menu"
        if (!sceneController.CurrentScene.name.Equals("Menu")) { Instantiate(playerUIPrefab); }

        _inputController.onSpacebarPressed += spacebarPressed;
    }

    private void spacebarPressed()
    {
        if(!isGameOver)
        {
            // CurrentPlayerHealth -= 10;
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

    // Events End
    private void OnDisable()
    {
        _inputController.onSpacebarPressed -= spacebarPressed;
    }
}
