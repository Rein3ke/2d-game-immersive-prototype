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
    public SoundController SoundController
    {
        get => _soundController;
    }

    private static GameController _currentGameController;

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

    private float currentPlayerHealth = 0.0f;
    private bool isPlayerbehindCover = false;
    private bool isGameOver = false;
    private bool isGameRunning = false;
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
        CurrentPlayerHealth = maxPlayerHealth;
        enemySpawnPosition = GameObject.FindGameObjectWithTag("Spawn_Enemy");

        // Instantiate Player UI and start game if current scene isn't "Main Menu"
        if (!_sceneController.CurrentScene.name.Equals("Menu")) {
            Instantiate(playerUIPrefab);
            isGameRunning = true;
            isGameOver = false;
            StartCoroutine(SpawnEnemies());
        }

        // Event Subscribtion
        _inputController.onSpacebarPressed  += spacebarPressed;
        _inputController.onSpacebarLeft += spacebarLeft;

        GunController.Instance.onRayCastHit += handleRayCastHit;
    }

    private IEnumerator SpawnEnemies()
    {
        while (isGameRunning && !isGameOver)
        {
            float waitForSeconds = Random.Range(5f, 10f);
            if (EnemyController.Count < maxEnemies)
            {
                GameObject enemy = Instantiate(enemyPrefab, enemySpawnPosition.transform.position, Quaternion.identity, null);
                enemy.GetComponent<EnemyController>().onEnemyDeath += handleEnemyDeath;
            }
            yield return new WaitForSeconds(waitForSeconds);
        }
        yield return null;
    }

    private void handleEnemyDeath(GameObject enemy)
    {
        enemy.GetComponent<EnemyController>().onEnemyDeath -= handleEnemyDeath;
    }

    /**
     * Handles the subscribed raycast hit (comes from an active camera controller)
     */
    private void handleRayCastHit(RaycastHit2D hit)
    {
        Collider2D hitCollider = hit.collider;
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
    public delegate void PlayerHealthCallback(float playerHealth);
    public event PlayerHealthCallback onPlayerHealthChange;
    public void PlayerHealthChange()
    {
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange(currentPlayerHealth);
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
        GunController.Instance.onRayCastHit -= handleRayCastHit;
    }
}
