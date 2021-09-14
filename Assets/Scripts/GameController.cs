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
        if (_sceneController == null) Debug.LogError("No scene controller!");
        _cursorController = GetComponent<CursorController>();
        _inputController = GetComponent<InputController>();
        _soundController = GetComponent<SoundController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        RunGameSetup();
    }

    private void RunGameSetup()
    {
        Debug.Log("GameController.RunGameSetup");

        if (_sceneController.CurrentScene.name.Equals("Level"))
        {
            switch (_currentState)
            {
                case State.DEFAULT:
                    Debug.Log("GameController.RunGameSetup.DEFAULT");
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
                case State.BLUR:
                    Debug.Log("GameController.RunGameSetup.BLUR");
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
                case State.PARTICLES:
                    Debug.Log("GameController.RunGameSetup.PARTICLES");
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
                case State.VISION:
                    Debug.Log("GameController.RunGameSetup.VISION");
                    Level.i.BuildLevel(GameAssets.i.levelPrefab_01, _currentState);
                    break;
            }
            Level.i.StartLevel();
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
}
