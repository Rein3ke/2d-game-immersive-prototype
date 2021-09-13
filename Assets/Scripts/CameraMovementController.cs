using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    private static CameraMovementController _instance;
    public static CameraMovementController Instance
    {
        get => _instance;
    }

    // Value that determines how far the camera can move down.
    [SerializeField]
    private float yOffset = 0.5f;

    // Value that determines how fast the camera can move horizontally.
    [SerializeField]
    private float movementSpeed = 4.0f;

    private bool isDown = false;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Debug.LogError("Error: Only one CameraController should be active at runtime.");
    }

    private void Start()
    {
        // Subscribed Events: Input
        GameController.CurrentGameController.InputController.onSpacebarDown += goDown;
        GameController.CurrentGameController.InputController.onSpacebarUp += goUp;
        GameController.CurrentGameController.InputController.onLeftDown += goLeft;
        GameController.CurrentGameController.InputController.onRightDown += goRight;
        GameController.CurrentGameController.InputController.onRightMouseDown += zoomIn;
        GameController.CurrentGameController.InputController.onRightMouseUp += zoomOut;
    }

    private void zoomIn()
    {
        Camera.main.fieldOfView -= 5f;
    }

    private void zoomOut()
    {
        Camera.main.fieldOfView += 5f;
    }

    private void goDown()
    {
        if (!Level.i.IsGameRunning || isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, -yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = true;
    }

    private void goUp()
    {
        if (!Level.i.IsGameRunning || !isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = false;
    }

    private void goLeft()
    {
        if (!Level.i.IsGameRunning) return;

        if (isDown)
        {
            transform.position += Vector3.left * (movementSpeed / 3) * Time.deltaTime;
        } else
        {
            transform.position += Vector3.left * movementSpeed * Time.deltaTime;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -5f, 5f), transform.position.y, transform.position.z);
    }

    private void goRight()
    {
        if (!Level.i.IsGameRunning) return;

        if (isDown)
        {
            transform.position += Vector3.right * (movementSpeed / 3) * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -5f, 5f), transform.position.y, transform.position.z);
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onSpacebarDown  -= goDown;
        GameController.CurrentGameController.InputController.onSpacebarUp     -= goUp;
        GameController.CurrentGameController.InputController.onLeftDown      -= goLeft;
        GameController.CurrentGameController.InputController.onRightDown     -= goRight;
        GameController.CurrentGameController.InputController.onRightMouseDown -= zoomIn;
        GameController.CurrentGameController.InputController.onRightMouseUp -= zoomOut;
    }
}
