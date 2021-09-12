using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum State
    {
        DEFAULT = 0,
        BLUR = 1,
        PARTICLES = 2,
        VISION = 3,
        Length = 4
    }

    // Own instance of GameController
    public static GameController CurrentGameController { get => _currentGameController; }
    private static GameController _currentGameController;

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

    private State _currentState = State.DEFAULT;

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
        if (GunController.i != null) GunController.i.onRayCastHit += OnRaycastHit;

        RunGameSetup();
    }

    private void RunGameSetup()
    {
        if (_sceneController.CurrentScene.name.Equals("Level"))
        {
            switch (_currentState)
            {
                case State.DEFAULT:
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
                case State.BLUR:
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
                case State.PARTICLES:
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
                case State.VISION:
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
            }
            Level.i.StartLevel();
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

    private State GetNextState()
    {
        int index = ((int)_currentState);
        string nextState = Enum.GetName(typeof(State), ++index);
        return (State) Enum.Parse(typeof(State), nextState);
    }

    private State GetStateByIndex(int index)
    {
        string nextState = Enum.GetName(typeof(State), index);
        return (State)Enum.Parse(typeof(State), nextState);
    }

    private void SetCurrentState(State state)
    {
        if (Equals(state, State.Length))
        {
            _sceneController.loadNextScene();
        } else
        {
            _currentState = state;
        }
    }

    public void loadNextLevel()
    {
        SetCurrentState(GetNextState());
        RunGameSetup();
    }

    public void retryLevel()
    {
        RunGameSetup();
    }

    public void loadMenu()
    {
        LoadingMainMenuScene();
    }

    #region Events
    public event Action onLoadingMainMenuScene;
    public void LoadingMainMenuScene()
    {
        if (onLoadingMainMenuScene != null)
        {
            Debug.Log("Event: Loading main menu scene!");
            onLoadingMainMenuScene();
        }
    }
    #endregion

    private void OnDisable()
    {
        if (GunController.i != null) GunController.i.onRayCastHit -= OnRaycastHit;
    }
}
