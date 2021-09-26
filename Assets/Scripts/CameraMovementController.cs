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

    private Vector3 _origin;
    private bool _isDown;
    private Camera _mainCamera;

    private void Awake()
    {
        // Prevent more than one camera controller
        if (Instance == null) Instance = this;
        else Debug.LogError("Error: Only one CameraController should be active at runtime.");
    }

    private void Start()
    {
        // Get reference to the main camera
        _mainCamera = Camera.main;
        
        if (_mainCamera == null)
        {
            Debug.LogError("No main camera found!");
            return;
        }
        
        // Set default position for reset purposes
        _origin = transform.position;
        
        // Event subscription
        GameController.Instance.InputController.onSpaceDown += InputController_OnSpaceDown;
        GameController.Instance.InputController.onSpaceUp += InputController_OnSpaceUp;
        GameController.Instance.InputController.onLeftDown += InputController_OnLeftDown;
        GameController.Instance.InputController.onRightDown += InputController_OnRightDown;
        GameController.Instance.InputController.onRightMouseDown += InputController_OnRightMouseDown;
        GameController.Instance.InputController.onRightMouseUp += InputController_OnRightMouseUp;
    }

    /// <summary>
    /// Starts the Zoom coroutine with the parameters needed to animate zoom in. Works only if game is running.
    /// </summary>
    private void InputController_OnRightMouseDown()
    {
        if (!Level.i.IsGameRunning) return;
        
        var fieldOfView = _mainCamera.fieldOfView;
        
        StartCoroutine(SmoothFieldOfViewValue(fieldOfView, fieldOfView - 2f, .2f));
    }

    /// <summary>
    /// Starts the Zoom coroutine with the parameters needed to animate zoom out. Works only if game is running.
    /// </summary>
    private void InputController_OnRightMouseUp()
    {
        if (!Level.i.IsGameRunning) return;
        
        var fieldOfView = _mainCamera.fieldOfView;
        
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
            _mainCamera.fieldOfView = Mathf.Lerp(vStart, vEnd, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        _mainCamera.fieldOfView = vEnd;
        
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
    private void InputController_OnSpaceDown()
    {
        if (!Level.i.IsGameRunning || _isDown) return;
        
        var transformPosition = transform.position;
        
        StartCoroutine(SmoothTransformPositionY(transformPosition.y, transformPosition.y - yOffset, .1f));
        
        _isDown = true;
    }

    /// <summary>
    /// Starts the coroutine to change the transform position.
    /// </summary>
    private void InputController_OnSpaceUp()
    {
        if (!Level.i.IsGameRunning || !_isDown) return;
        
        var transformPosition = transform.position;
        
        StartCoroutine(SmoothTransformPositionY(transformPosition.y, _origin.y, .1f));
        
        _isDown = false;
    }

    /// <summary>
    /// Moves the Main Camera to the left.
    /// </summary>
    private void InputController_OnLeftDown()
    {
        if (!Level.i.IsGameRunning) return;

        if (_isDown)
        {
            transform.position += Vector3.left * ((movementSpeed / 3) * Time.deltaTime);
        } else
        {
            transform.position += Vector3.left * (movementSpeed * Time.deltaTime);
        }

        var transformPosition = transform.position;
        transformPosition = new Vector3(Mathf.Clamp(transformPosition.x, -5f, 5f), transformPosition.y, transformPosition.z);
        transform.position = transformPosition;
    }

    /// <summary>
    /// Moves the Main Camera to the right.
    /// </summary>
    private void InputController_OnRightDown()
    {
        if (!Level.i.IsGameRunning) return;

        if (_isDown)
        {
            transform.position += Vector3.right * ((movementSpeed / 3) * Time.deltaTime);
        }
        else
        {
            transform.position += Vector3.right * (movementSpeed * Time.deltaTime);
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
        GameController.Instance.InputController.onSpaceDown -= InputController_OnSpaceDown;
        GameController.Instance.InputController.onSpaceUp -= InputController_OnSpaceUp;
        GameController.Instance.InputController.onLeftDown -= InputController_OnLeftDown;
        GameController.Instance.InputController.onRightDown -= InputController_OnRightDown;
        GameController.Instance.InputController.onRightMouseDown -= InputController_OnRightMouseDown;
        GameController.Instance.InputController.onRightMouseUp -= InputController_OnRightMouseUp;
    }
}
