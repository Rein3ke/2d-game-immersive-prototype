using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public enum Phase
    {
        DEFAULT, BLUR, PARTICLES, VISION
    }

    // Own instance of GameController
    public static GameController CurrentGameController { get => _currentGameController; }
    private static GameController _currentGameController;

    public bool IsGameOver
    {
        get => _isGameOver;
    }
    private bool _isGameOver = false;

    // Public access for the input and sound controller
    public InputController InputController
    {
        get => _inputController;
    }
    private InputController _inputController;
    public SoundController SoundController
    {
        get => _soundController;
    }
    private SoundController _soundController;

    // All Controllers
    private SceneController _sceneController;
    private CursorController _cursorController;
    private Level _level;

    private Phase currentPhase = Phase.DEFAULT;

    private void Awake()
    {
        if (_currentGameController != null && _currentGameController != this)
        {
            Destroy(gameObject);
        } else
        {
            _currentGameController = this;
        }

        // Set all controller
        _sceneController = GetComponent<SceneController>();
        _cursorController = GetComponent<CursorController>();
        _inputController = GetComponent<InputController>();
        _soundController = GetComponent<SoundController>();
        _level = GetComponent<Level>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get Enemy Spawn Position (as GameObject)
        // _enemySpawnPosition = GameObject.FindGameObjectWithTag("Spawn_Enemy");
        /*
        // Instantiate Player UI and start game if current scene isn't "Main Menu"
        if (!_sceneController.CurrentScene.name.Equals("Menu")) {
            Instantiate(playerUIPrefab, new Vector3(0f, 0f, -20f), Quaternion.identity);

            _isGameRunning = true;
            _isGameOver = false;
            playerSettings.OnEnable();

            StartCoroutine(SpawnEnemies());
        } else
        {
            _isGameRunning = false;
        }
        */
        // Event Subscribtion
        if (_sceneController.CurrentScene.name.Equals("Level"))
        {
            switch(currentPhase)
            {
                case Phase.DEFAULT:
                    _level.BuildLevel(GameAssets.i.levelPrefab_01, currentPhase);
                    break;
            }
            _level.StartLevel();
        }

        if (GunController.Instance != null) GunController.Instance.onRayCastHit += OnRaycastHit;
    }

    public void AddToScore(float value)
    {
        GameAssets.i.playerSettings_default.score += value;
        ScoreChange();

        CheckForWinConditions();
    }

    // Processes the subscribed raycast hit and forwards the actual hit command. (Comes from an active camera controller)
    private void OnRaycastHit(RaycastHit2D hitObject)
    {
        Collider2D hitCollider = hitObject.collider;
        switch (hitCollider.tag)
        {
            case "InteractableObject":
                hitCollider.gameObject.GetComponent<InteractableObjectController>().handleHit();
                break;
            case "Enemy":
                hitCollider.gameObject.GetComponent<EnemyController>().handleHit();
                break;
        }
    }

    private bool CheckForWinConditions()
    {
        if (GameAssets.i.playerSettings_default.score >= GameAssets.i.gameSettings_default.ScoreToBeAchieved)
        {
            GameWon();
            return true;
        } else
        {
            return false;
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

    #region Events
    public event Action onGameEnd;
    public void GameEnd()
    {
        if (onGameEnd != null)
        {
            Debug.Log("Event: Player death!");
            onGameEnd();
        }
    }

    public event Action onScoreChange;
    public void ScoreChange()
    {
        if (onScoreChange != null)
        {
            onScoreChange();
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

    public event Action onGameWon;
    public void GameWon()
    {
        if (onGameWon != null)
        {
            onGameWon();
        }
    }
    #endregion

    private void OnDisable()
    {
        if (GunController.Instance != null) GunController.Instance.onRayCastHit -= OnRaycastHit;
    }
}
