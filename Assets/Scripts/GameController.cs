using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController CurrentGameController { get => _currentGameController; }

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

    // Own instance of GameController
    private static GameController _currentGameController;

    // All Controllers
    private SceneController _sceneController;
    private CursorController _cursorController;
    private InputController _inputController;
    private SoundController _soundController;

    [SerializeField]
    private GameObject playerUIPrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float maxPlayerHealth = 100.0f;
    [SerializeField]
    private int maxEnemies = 2;
    [SerializeField]
    private PlayerSettings playerSettings;

    private bool isGameRunning = false;
    private bool isPlayerbehindCover = false;
    private GameObject enemySpawnPosition;

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

            playerSettings.playerHealth = playerSettings.playerMaxHealth;
            PlayerHealthChange();

            StartCoroutine(SpawnEnemies());
        } else
        {
            isGameRunning = false;
        }

        // Event Subscribtion
        _inputController.onSpacebarPressed  += spacebarPressed;
        _inputController.onSpacebarLeft += spacebarLeft;
        GunController.Instance.onRayCastHit += OnRaycastHit;
    }

    // Coroutine: When the game is running and it's not Game Over, start spawning enemies.
    private IEnumerator SpawnEnemies()
    {
        while (isGameRunning && !isGameOver)
        {
            float waitForSeconds = Random.Range(2f, 8f);
            yield return new WaitForSeconds(waitForSeconds);

            // Do not spawn more enemies once the maximum number of enemies is reached.
            if (EnemyController.Count < maxEnemies && (isGameRunning && !isGameOver))
            {
                GameObject enemy = Instantiate(enemyPrefab, enemySpawnPosition.transform.position, Quaternion.identity, null);
                enemy.GetComponent<EnemyController>().onEnemyDeath += OnEnemyDeath;
                enemy.GetComponent<EnemyController>().onPlayerHit += OnPlayerHit;
            }
        }
        yield return null;
    }

    // Player & Game Logic

    // Subscribed Event: Handle the case when an enemy dies.
    private void OnEnemyDeath(GameObject enemy)
    {
        enemy.GetComponent<EnemyController>().onEnemyDeath -= OnEnemyDeath;
        enemy.GetComponent<EnemyController>().onPlayerHit -= OnPlayerHit;
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
            PlayerDeath();
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

    private void spacebarPressed()
    {
        if (isGameOver) return;

        isPlayerbehindCover = true;
    }
    private void spacebarLeft()
    {
        if (isGameOver) return;

        isPlayerbehindCover = false;
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
        _inputController.onSpacebarPressed  -= spacebarPressed;
        GunController.Instance.onRayCastHit -= OnRaycastHit;
    }
}
