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
        get => isGameOver;
    }
    private bool isGameOver = false;

    // Public access for the input and sound controller
    public InputController InputController
    {
        get => _inputController;
    }
    public SoundController SoundController
    {
        get => _soundController;
    }

    // All Controllers
    private SceneController _sceneController;
    private CursorController _cursorController;
    private InputController _inputController;
    private SoundController _soundController;

    [SerializeField]
    private GameObject playerUIPrefab;
    [SerializeField]
    private PlayerSettings playerSettings;
    [SerializeField]
    private GameSettings gameSettings;

    private bool isGameRunning = false;
    private GameObject enemySpawnPosition;
    private int killedEnemies = 0;
    private Phase currentPhase = Phase.DEFAULT;

    private void Awake()
    {
        if (_currentGameController != null && _currentGameController != this)
        {
            Destroy(this.gameObject);
        } else
        {
            _currentGameController = this;
        }

        // Set all controller
        _sceneController = GetComponent<SceneController>();
        _cursorController = GetComponent<CursorController>();
        _inputController = GetComponent<InputController>();
        _soundController = GetComponent<SoundController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get Enemy Spawn Position (as GameObject)
        enemySpawnPosition = GameObject.FindGameObjectWithTag("Spawn_Enemy");

        // Instantiate Player UI and start game if current scene isn't "Main Menu"
        if (!_sceneController.CurrentScene.name.Equals("Menu")) {
            Instantiate(playerUIPrefab);

            isGameRunning = true;
            isGameOver = false;

            StartCoroutine(SpawnEnemies());
        } else
        {
            isGameRunning = false;
        }

        // Event Subscribtion
        GunController.Instance.onRayCastHit += OnRaycastHit;
    }

    // Coroutine: When the game is running and it's not Game Over, start spawning enemies.
    private IEnumerator SpawnEnemies()
    {
        while (isGameRunning && !isGameOver)
        {
            float waitForSeconds = Random.Range(gameSettings.enemySpawnMinimumCooldown, gameSettings.enemySpawnMaximumCooldown);
            yield return new WaitForSeconds(waitForSeconds);

            // Do not spawn more enemies once the maximum number of enemies is reached.
            if (EnemyController.Count < gameSettings.maxEnemies && (isGameRunning && !isGameOver))
            {
                GameObject enemyPrefab = gameSettings.enemyPrefabs[Random.Range(0, gameSettings.enemyPrefabs.Count)];
                GameObject enemy = Instantiate(enemyPrefab, enemySpawnPosition.transform.position, Quaternion.identity, null);
                enemy.GetComponent<EnemyController>().onEnemyDeath += OnEnemyDeath;
                enemy.GetComponent<EnemyController>().onPlayerHit += OnPlayerHit;
            }
        }
        yield return null;
    }

    // Player & Game Logic

    // Subscribed Event: Handle the case when an enemy dies.
    private void OnEnemyDeath(GameObject enemy, float score)
    {
        killedEnemies++;
        AddToScore(score);

        enemy.GetComponent<EnemyController>().onEnemyDeath -= OnEnemyDeath;
        enemy.GetComponent<EnemyController>().onPlayerHit -= OnPlayerHit;
    }

    private void AddToScore(float value)
    {
        playerSettings.score += value;
        ScoreChange();

        CheckForWinConditions();
    }

    private void OnPlayerHit(float damage)
    {
        playerSettings.playerHealth -= damage;
        PlayerHealthChange();
        CheckIsPlayerDead();
    }

    private void CheckIsPlayerDead()
    {
        if (playerSettings.playerHealth <= 0.0f)
        {
            GameEnd();
            isGameOver = true;
        }
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
        if ((playerSettings.score >= gameSettings.ScoreToBeAchieved) || (killedEnemies > gameSettings.EnemiesToKill))
        {
            isGameRunning = false;
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
    public event Action onPlayerHealthChange;
    public void PlayerHealthChange()
    {
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange();
        }
    }

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
        GunController.Instance.onRayCastHit -= OnRaycastHit;
    }
}
