using System.Collections;
using UnityEngine;

/// <summary>
/// Controls the main camera in the scene.
/// </summary>
public class CameraMovementController : MonoBehaviour
{
    public static CameraMovementController Instance { get; private set; }

    // Value that determines how far the camera can move down.
    [SerializeField] private float yOffset = 0.5f;

    // Value that determines how fast the camera can move horizontally.
    [SerializeField] private float movementSpeed = 4.0f;

    private Vector3 m_origin;
    private bool m_isDown;
    private Camera m_mainCamera;

    private void Awake()
    {
        // Prevent more than one camera controller
        if (Instance == null) Instance = this;
        else Debug.LogError("Error: Only one CameraController should be active at runtime.");
    }

    private void Start()
    {
        // Get reference to the main camera
        m_mainCamera = Camera.main;
        
        if (m_mainCamera == null)
        {
            Debug.LogError("No main camera found!");
            return;
        }
        
        // Set default position for reset purposes
        m_origin = transform.position;
        
        // Event subscription
        GameController.CurrentGameController.InputController.onSpacebarDown += GoDown;
        GameController.CurrentGameController.InputController.onSpacebarUp += GoUp;
        GameController.CurrentGameController.InputController.onLeftDown += GoLeft;
        GameController.CurrentGameController.InputController.onRightDown += GoRight;
        GameController.CurrentGameController.InputController.onRightMouseDown += ZoomIn;
        GameController.CurrentGameController.InputController.onRightMouseUp += ZoomOut;
    }

    /// <summary>
    /// Starts the Zoom coroutine with the parameters needed to animate zoom in. Works only if game is running.
    /// </summary>
    private void ZoomIn()
    {
        if (!Level.i.IsGameRunning) return;
        
        var fieldOfView = m_mainCamera.fieldOfView;
        
        StartCoroutine(SmoothFieldOfViewValue(fieldOfView, fieldOfView - 2f, .2f));
    }

    /// <summary>
    /// Starts the Zoom coroutine with the parameters needed to animate zoom out. Works only if game is running.
    /// </summary>
    private void ZoomOut()
    {
        if (!Level.i.IsGameRunning) return;
        
        var fieldOfView = m_mainCamera.fieldOfView;
        
        StartCoroutine(SmoothFieldOfViewValue(fieldOfView, fieldOfView + 2f, .2f));
    }
    
    /// <summary>
    /// Coroutine: Animates the field of view from start to end value over a set time.
    /// </summary>
    /// <param name="vStart">Start value</param>
    /// <param name="vEnd">Final value</param>
    /// <param name="duration">Duration in seconds</param>
    /// <returns>Nothing</returns>
    private IEnumerator SmoothFieldOfViewValue(float vStart, float vEnd, float duration)
    {
        float elapsed = 0.0f;
        
        while (elapsed < duration)
        {
            m_mainCamera.fieldOfView = Mathf.Lerp(vStart, vEnd, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        m_mainCamera.fieldOfView = vEnd;
        
        yield return null;
    }

    /// <summary>
    /// Coroutine: Animates the y value of transform position from start to end value over a set time.
    /// </summary>
    /// <param name="vStart">Start value</param>
    /// <param name="vEnd">Final value</param>
    /// <param name="duration">Duration in seconds</param>
    /// <returns>Nothing</returns>
    private IEnumerator SmoothTransformPositionY(float vStart, float vEnd, float duration)
    {
        float elapsed = 0.0f;
        
        while (elapsed < duration)
        {
            var transformPosition = transform.position;
            transformPosition = new Vector3(transformPosition.x, Mathf.Lerp(vStart, vEnd, elapsed / duration), transformPosition.z);
            transform.position = transformPosition;
            elapsed += Time.deltaTime;
            yield return null;
        }

        var currentTransform = transform;
        
        var finalPosition = currentTransform.position;
        
        finalPosition = new Vector3(finalPosition.x, vEnd, finalPosition.z);
        
        currentTransform.position = finalPosition;
        
        yield return null;
    }

    /// <summary>
    /// Starts the coroutine to change the transform position.
    /// </summary>
    private void GoDown()
    {
        if (!Level.i.IsGameRunning || m_isDown) return;
        
        var transformPosition = transform.position;
        
        StartCoroutine(SmoothTransformPositionY(transformPosition.y, transformPosition.y - yOffset, .1f));
        
        m_isDown = true;
    }

    /// <summary>
    /// Starts the coroutine to change the transform position.
    /// </summary>
    private void GoUp()
    {
        if (!Level.i.IsGameRunning || !m_isDown) return;
        
        var transformPosition = transform.position;
        
        StartCoroutine(SmoothTransformPositionY(transformPosition.y, m_origin.y, .1f));
        
        m_isDown = false;
    }

    /// <summary>
    /// Moves the Main Camera to the left.
    /// </summary>
    private void GoLeft()
    {
        if (!Level.i.IsGameRunning) return;

        if (m_isDown)
        {
            transform.position += Vector3.left * (movementSpeed / 3) * Time.deltaTime;
        } else
        {
            transform.position += Vector3.left * movementSpeed * Time.deltaTime;
        }

        var transformPosition = transform.position;
        transformPosition = new Vector3(Mathf.Clamp(transformPosition.x, -5f, 5f), transformPosition.y, transformPosition.z);
        transform.position = transformPosition;
    }

    /// <summary>
    /// Moves the Main Camera to the right.
    /// </summary>
    private void GoRight()
    {
        if (!Level.i.IsGameRunning) return;

        if (m_isDown)
        {
            transform.position += Vector3.right * (movementSpeed / 3) * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }

        var transformPosition = transform.position;
        
        transformPosition = new Vector3(Mathf.Clamp(transformPosition.x, -5f, 5f), transformPosition.y, transformPosition.z);
        
        transform.position = transformPosition;
    }

    /// <summary>
    /// Unsubscribe from all events.
    /// </summary>
    private void OnDestroy()
    {
        GameController.CurrentGameController.InputController.onSpacebarDown  -= GoDown;
        GameController.CurrentGameController.InputController.onSpacebarUp     -= GoUp;
        GameController.CurrentGameController.InputController.onLeftDown      -= GoLeft;
        GameController.CurrentGameController.InputController.onRightDown     -= GoRight;
        GameController.CurrentGameController.InputController.onRightMouseDown -= ZoomIn;
        GameController.CurrentGameController.InputController.onRightMouseUp -= ZoomOut;
    }
}
