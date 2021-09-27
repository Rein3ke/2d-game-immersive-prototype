using System;
using System.Collections;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    public static Level i
    {
        get => _i;
    }
    private static Level _i;

    private GameSettings _gameSettings;
    private PlayerSettings _playerSettings;
    private GameController _gameController;
    private MaterialChangeController _materialChangeController;

    public bool IsGameRunning
    {
        get => _isGameRunning;
    }
    private bool _isGameRunning;
    private GameObject _activeLevelPrefab;
    private Coroutine _currentSpawnEnemiesRoutine;
    private Coroutine _currentSpawnInteractablesRoutine;

    private void Awake()
    {
        if (_i != null) Debug.LogError("Too many level controllers!");
        else _i = this;

        _gameController = GameController.Instance;
        _materialChangeController = GetComponent<MaterialChangeController>();
        if (_materialChangeController == null) Debug.LogError("Error: No MaterialChangeController found!");
    }

    public void BuildLevel(GameObject levelPrefab, GameController.State state)
    {
        ResetLevel();

        _activeLevelPrefab = Instantiate(levelPrefab);

        PlayerUIController playerUIController = FindObjectOfType<PlayerUIController>();
        if (FindObjectOfType<PlayerUIController>() == null)
        {
             playerUIController = Instantiate(GameAssets.I.playerUIPrefab).GetComponentInChildren<PlayerUIController>();
        }

        switch (state)
        {
            case GameController.State.DEFAULT:
                _playerSettings = GameAssets.I.playerSettings_default;
                _gameSettings = GameAssets.I.gameSettings_default;
                InstantiateTutorial(true, true);
                break;
            case GameController.State.BLUR:
                _playerSettings = GameAssets.I.playerSettings_default;
                _gameSettings = GameAssets.I.gameSettings_default;
                break;
            case GameController.State.DOODLE:
                _playerSettings = GameAssets.I.playerSettings_default;
                _gameSettings = GameAssets.I.gameSettings_default;
                break;
            case GameController.State.VISION:
                _playerSettings = GameAssets.I.playerSettings_default;
                _gameSettings = GameAssets.I.gameSettings_default;
                InstantiateTutorial(true, false);
                break;
            default:
                Debug.LogError("No feature set!");
                break;
        }

        playerUIController.PlayerSettings = _playerSettings;
        playerUIController.GameSettings = _gameSettings;

        GunController.I.PlayerSettings = _playerSettings;
        GameController.Instance.InputController.PlayerSettings = _playerSettings;
        _materialChangeController.GameSettings = _gameSettings;

        _gameSettings.state = state;
        StateChange();
    }

    private void InstantiateTutorial(bool showRightMouseButton, bool showSpacebar)
    {
        var tutorialUI = Instantiate(Resources.Load("TutorialUICanvas") as GameObject);
        
        var tutorialUIController = tutorialUI.GetComponent<TutorialUIController>();
        if (tutorialUIController == null) return;
        
        tutorialUIController.PlayerSettings = _playerSettings;
        if (showRightMouseButton) tutorialUIController.ShowRightMouseButton = true;
        if (showSpacebar) tutorialUIController.ShowSpacebar = true;
    }

    private void ResetLevel()
    {
        if (_currentSpawnEnemiesRoutine != null)
        {
            StopCoroutine(_currentSpawnEnemiesRoutine);
            _currentSpawnEnemiesRoutine = null;
        }
        if (_currentSpawnInteractablesRoutine != null)
        {
            StopCoroutine(_currentSpawnInteractablesRoutine);
            _currentSpawnInteractablesRoutine = null;
        }

        // Delete current level
        if (_activeLevelPrefab != null) Destroy(_activeLevelPrefab);

        // Remove all active enemies
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        // Remove all active interactive objects
        InteractableObjectController[] interactables = FindObjectsOfType<InteractableObjectController>();
        foreach (InteractableObjectController interactable in interactables)
        {
            Destroy(interactable.gameObject);
        }

        // Reset Enemy Counter (used for Enemy Spawning)
        EnemyController.Count = 0;
        
        // Delete Tutorial UI GameObject & Reset Counters
        var tutorialUIGameObject = FindObjectOfType<TutorialUIController>()?.gameObject;
        if (tutorialUIGameObject != null) Destroy(tutorialUIGameObject);
        if (_playerSettings != null)
        {
            _playerSettings.SpacebarPressedCount = 0;
            _playerSettings.RightClickedCount = 0;
        }
    }

    public void StartLevel()
    {
        _isGameRunning = true;

        // Player Settings Reset
        _playerSettings.OnEnable();

        _currentSpawnEnemiesRoutine = StartCoroutine(SpawnEnemies());
        _currentSpawnInteractablesRoutine = StartCoroutine(SpawnInteractables());
    }

    private IEnumerator SpawnEnemies()
    {
        while (_isGameRunning)
        {
            float waitForSeconds = Random.Range(_gameSettings.enemySpawnMinimumCooldown, _gameSettings.enemySpawnMaximumCooldown);
            yield return new WaitForSeconds(waitForSeconds);

            // Do not spawn more enemies once the maximum number of enemies is reached.
            if (EnemyController.Count < _gameSettings.maxEnemies && _isGameRunning)
            {
                GameObject enemyPrefab = _gameSettings.enemyPrefabs[Random.Range(0, _gameSettings.enemyPrefabs.Count)];
                GameObject enemy = Instantiate(enemyPrefab, GetRandomEnemySpawnPosition(), Quaternion.identity, null);
                // Enemy Events
                enemy.GetComponent<EnemyController>().onEnemyDeath += OnEnemyDeath;
                enemy.GetComponent<EnemyController>().onPlayerHit += OnPlayerHit;
            }
        }
        yield return null;
    }

    private IEnumerator SpawnInteractables()
    {
        while (_isGameRunning)
        {
            float waitForSeconds = Random.Range(_gameSettings.objectSpawnMinimumCooldown, _gameSettings.objectSpawnMaximumCooldown);
            yield return new WaitForSeconds(waitForSeconds);

            GameObject interactablePrefab = _gameSettings.spawnableGameObjects[Random.Range(0, _gameSettings.spawnableGameObjects.Count)];
            GameObject interactableGameObject = Instantiate(interactablePrefab, GetRandomInteractableSpawnPosition(), Quaternion.identity, null);

            Destroy(interactableGameObject, waitForSeconds);
            yield return null;
        }
        yield return null;
    }

    private Vector3 GetRandomInteractableSpawnPosition()
    {
        GameObject[] objectSpawns = GameObject.FindGameObjectsWithTag("InteractableObject_Position");
        if (objectSpawns.Length > 0)
        {
            return objectSpawns[Random.Range(0, objectSpawns.Length)].transform.position;
        }
        else
        {
            Debug.LogError("Error: No object spawns found!");
            return Vector3.zero;
        }
    }

    private Vector3 GetRandomEnemySpawnPosition()
    {
        GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("Spawn_Enemy");
        if (enemySpawns.Length > 0)
        {
            return enemySpawns[Random.Range(0, enemySpawns.Length)].transform.position;
        } else
        {
            Debug.LogError("Error: No enemy spawns found!");
            return Vector3.zero;
        }
    }

    public void AddToScore(float value)
    {
        _playerSettings.Score += value;
        ScoreChange();

        if (CheckScoreWinCondition())
        {
            _isGameRunning = false;
            GameWon();
        }
    }

    private bool CheckIsPlayerAlive()
    {
        if (_playerSettings.PlayerHealth <= 0.0f)
        {
            return false;
        } else
        {
            return true;
        }
    }

    private bool CheckScoreWinCondition()
    {
        if (_playerSettings.Score >= _gameSettings.ScoreToBeAchieved)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal void TakeDamage(float damage)
    {
        _playerSettings.PlayerHealth -= damage;
        _playerSettings.PlayerHealth = Mathf.Clamp(_playerSettings.PlayerHealth, 0.0f, _playerSettings.playerMaxHealth);
        PlayerHealthChange();

        if (!CheckIsPlayerAlive())
        {
            _isGameRunning = false;
            GameLost();
        }
    }


    #region Event Handling
    private void OnPlayerHit(float damage)
    {
        TakeDamage(damage);
    }

    private void OnEnemyDeath(GameObject enemy, float score)
    {
        AddToScore(score);

        enemy.GetComponent<EnemyController>().onEnemyDeath -= OnEnemyDeath;
        enemy.GetComponent<EnemyController>().onPlayerHit -= OnPlayerHit;
    }
    #endregion

    #region Events
    public event Action onPlayerHealthChange;
    public void PlayerHealthChange()
    {
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange();
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

    public event Action onGameWon;
    public void GameWon()
    {
        if (onGameWon != null)
        {
            onGameWon();
        }
    }

    public event Action onGameLost;
    public void GameLost()
    {
        if (onGameLost != null)
        {
            onGameLost();
        }
    }

    public event Action onStateChange;
    public void StateChange()
    {
        if (onStateChange != null)
        {
            onStateChange();
        }
    }
    #endregion
}
