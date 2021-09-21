using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public enum State
    {
        DEFAULT = 0,
        BLUR = 1,
        DOODLE = 2,
        VISION = 3,
        Length = 4
    }

    // Own instance of GameController
    public static GameController CurrentGameController { get => m_currentGameController; private set => m_currentGameController = value; }
    private static GameController m_currentGameController;

    // All Controllers

    // Public access for the input and sound controller
    public InputController InputController { get => m_inputController; private set => m_inputController = value; }
    private InputController m_inputController;

    public SceneController SceneController { get => m_sceneController; private set => m_sceneController = value; }
    private SceneController m_sceneController;
    private CursorController m_cursorController;
    public SoundController SoundController { get => m_soundController; private set => m_soundController = value; }
    private SoundController m_soundController;

    private State m_currentState = State.DEFAULT;

    private void Awake()
    {
        if (CurrentGameController != null && CurrentGameController != this)
        {
            Destroy(gameObject);
        } else
        {
            CurrentGameController = this;
        }

        // Set all controller
        SceneController = GetComponent<SceneController>();
        if (SceneController == null) Debug.LogError("No scene controller!");
        m_cursorController = GetComponent<CursorController>();
        InputController = GetComponent<InputController>();
        SoundController = GetComponent<SoundController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        RunGameSetup();
        m_inputController.onKeyEscapeDown += OnKeyEscapeDown;
    }

    private void RunGameSetup()
    {
        if (!m_sceneController.CurrentScene.name.Equals("Level")) return;
        Debug.Log("GameController.RunGameSetup");

        switch (m_currentState)
        {
            default:
            case State.DEFAULT:
                Debug.Log("GameController.RunGameSetup.DEFAULT");
                Level.i.BuildLevel(GameAssets.i.levelPrefab_01, m_currentState);
                break;
            case State.BLUR:
                Debug.Log("GameController.RunGameSetup.BLUR");
                Level.i.BuildLevel(GameAssets.i.levelPrefab_01, m_currentState);
                break;
            case State.DOODLE:
                Debug.Log("GameController.RunGameSetup.DOODLE");
                Level.i.BuildLevel(GameAssets.i.levelPrefab_01, m_currentState);
                break;
            case State.VISION:
                Debug.Log("GameController.RunGameSetup.VISION");
                Level.i.BuildLevel(GameAssets.i.levelPrefab_01, m_currentState);
                break;
        }
        Level.i.StartLevel();
    }

    private State GetNextState()
    {
        int index = ((int)m_currentState);
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
            m_sceneController.loadNextScene();
        } else
        {
            m_currentState = state;
        }
    }

    public void LoadNextLevel()
    {
        SetCurrentState(GetNextState());
        RunGameSetup();
    }

    public void RetryLevel()
    {
        RunGameSetup();
    }

    public void LoadMenu()
    {
        LoadingMainMenuScene();
    }

    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #region Event Handling
    private static void OnKeyEscapeDown()
    {
        ExitGame();
    }
#endregion

    #region Events
    public event Action onLoadingMainMenuScene;
    public void LoadingMainMenuScene()
    {
        onLoadingMainMenuScene?.Invoke();
    }
    #endregion
}
