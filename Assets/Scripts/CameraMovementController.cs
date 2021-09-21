using System.Collections;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    public static CameraMovementController Instance { get; private set; }

    // Value that determines how far the camera can move down.
    [SerializeField]
    private float yOffset = 0.5f;

    // Value that determines how fast the camera can move horizontally.
    [SerializeField]
    private float movementSpeed = 4.0f;

    private Vector3 m_origin;
    private bool m_isDown = false;
    private Camera m_mainCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Debug.LogError("Error: Only one CameraController should be active at runtime.");
    }

    private void Start()
    {
        m_mainCamera = Camera.main;
        if (m_mainCamera == null)
        {
            Debug.LogError("No main camera found!");
            return;
        }
        m_origin = transform.position;
        // Subscribed Events: Input
        GameController.CurrentGameController.InputController.onSpacebarDown += GoDown;
        GameController.CurrentGameController.InputController.onSpacebarUp += GoUp;
        GameController.CurrentGameController.InputController.onLeftDown += GoLeft;
        GameController.CurrentGameController.InputController.onRightDown += GoRight;
        GameController.CurrentGameController.InputController.onRightMouseDown += ZoomIn;
        GameController.CurrentGameController.InputController.onRightMouseUp += ZoomOut;
    }

    private void ZoomIn()
    {
        if (!Level.i.IsGameRunning) return;
        
        var fieldOfView = m_mainCamera.fieldOfView;
        StartCoroutine(Zoom(fieldOfView, fieldOfView - 2f, .2f));
    }

    private void ZoomOut()
    {
        if (!Level.i.IsGameRunning) return;
        
        var fieldOfView = m_mainCamera.fieldOfView;
        StartCoroutine(Zoom(fieldOfView, fieldOfView + 2f, .2f));
    }

    private IEnumerator Zoom(float vStart, float vEnd, float duration)
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

    private IEnumerator GoUpOrDown(float vStart, float vEnd, float duration)
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

    private void GoDown()
    {
        if (!Level.i.IsGameRunning || m_isDown) return;

        //Vector3 currentPosition = transform.position;
        //transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, -yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        var transformPosition = transform.position;
        StartCoroutine(GoUpOrDown(transformPosition.y, transformPosition.y - yOffset, .1f));
        m_isDown = true;
    }

    private void GoUp()
    {
        if (!Level.i.IsGameRunning || !m_isDown) return;

        //Vector3 currentPosition = transform.position;
        //transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        var transformPosition = transform.position;
        StartCoroutine(GoUpOrDown(transformPosition.y, m_origin.y, .1f));
        m_isDown = false;
    }

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
