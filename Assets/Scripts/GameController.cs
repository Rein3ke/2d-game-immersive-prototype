using System;
using Assets;
using UnityEngine;

[RequireComponent(typeof(InputController), typeof(SoundController), typeof(SceneController))]
public class GameController : MonoBehaviour
{
    /// <summary>
    /// An enum used to set the current application state.
    /// </summary>
    public enum State
    {
        DEFAULT = 0,
        BLUR = 1,
        DOODLE = 2,
        VISION = 3,
        Length = 4
    }

    // Own instance of GameController
    public static GameController Instance { get; private set; }

    // All Controllers

    // Public access for the input, scene and sound controller
    public InputController InputController { get; private set; }
    public SceneController SceneController { get; private set; }
    public SoundController SoundController { get; private set; }

    private State _currentState = State.DEFAULT;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
        }

        // Set all controller
        SceneController = GetComponent<SceneController>();
        if (SceneController == null) Debug.LogError("No scene controller!");

        InputController = GetComponent<InputController>();
        
        SoundController = GetComponent<SoundController>();
    }
    
    /// <summary>
    /// Standard unity method. Executes the RunGameSetup() method.
    /// </summary>
    private void Start()
    {
        RunGameSetup();
        
        InputController.onKeyEscapeDown += InputController_OnKeyEscapeDown;
    }

    /// <summary>
    /// Configures the level script depending on the current state. After configuration the level is started.
    /// </summary>
    private void RunGameSetup()
    {
        if (!SceneController.CurrentScene.name.Equals("Level")) return;

        switch (_currentState)
        {
            default:
            case State.DEFAULT:
                Level.I.BuildLevel(GameAssets.I.levelPrefab_01, _currentState);
                break;
            case State.BLUR:
                Level.I.BuildLevel(GameAssets.I.levelPrefab_01, _currentState);
                break;
            case State.DOODLE:
                Level.I.BuildLevel(GameAssets.I.levelPrefab_01, _currentState);
                break;
            case State.VISION:
                Level.I.BuildLevel(GameAssets.I.levelPrefab_01, _currentState);
                break;
        }
        Level.I.StartLevel();
    }

    /// <summary>
    /// Returns the next state depending on the enum.
    /// </summary>
    /// <returns>Next state depending on the enum</returns>
    private State GetNextState()
    {
        int index = ((int)_currentState);
        
        string nextState = Enum.GetName(typeof(State), ++index);
        
        return (State) Enum.Parse(typeof(State), nextState);
    }

    /// <summary>
    /// Takes an integer and returns a corresponding state.
    /// </summary>
    /// <param name="index">State index ID</param>
    /// <returns>State</returns>
    private State GetStateByIndex(int index)
    {
        string nextState = Enum.GetName(typeof(State), index);
        return (State)Enum.Parse(typeof(State), nextState);
    }

    /// <summary>
    /// Sets the next active state.
    /// </summary>
    /// <param name="state">Active next state</param>
    private void SetCurrentState(State state)
    {
        if (Equals(state, State.Length))
        {
            SceneController.LoadNextScene();
        } else
        {
            _currentState = state;
        }
    }

    /// <summary>
    /// Switches to the next state. Executes the RunGameSetup() method.
    /// </summary>
    public void LoadNextLevel()
    {
        SetCurrentState(GetNextState());
        
        RunGameSetup();
    }

    /// <summary>
    /// Public access for UI elements: Starts the RunGameSetup() method.
    /// </summary>
    public void RetryLevel()
    {
        RunGameSetup();
    }

    /// <summary>
    /// Public access for UI elements: Loads the main menu scene.
    /// </summary>
    public void LoadMenu()
    {
        LoadingMainMenuScene();
    }

    /// <summary>
    /// Handles the application close request. Whether in the editor or in the build, the application closes differently.
    /// </summary>
    public static void CloseApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Event Handling
    private static void InputController_OnKeyEscapeDown()
    {
        CloseApplication();
    }
#endregion

    #region Events
    public event Action onLoadingMainMenuScene;
    private void LoadingMainMenuScene()
    {
        onLoadingMainMenuScene?.Invoke();
    }
    #endregion
}
