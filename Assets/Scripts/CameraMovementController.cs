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
    private bool isActive = true;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Debug.LogError("Error: Only one CameraController should be active at runtime.");
    }

    private void Start()
    {
        // Subscribed Events: Input
        GameController.CurrentGameController.InputController.onSpacebarPressed += goDown;
        GameController.CurrentGameController.InputController.onSpacebarLeft += goUp;
        GameController.CurrentGameController.InputController.onLeftPressed += goLeft;
        GameController.CurrentGameController.InputController.onRightPressed += goRight;

        // Subscribed Events: Game State
        GameController.CurrentGameController.onPlayerDeath += OnPlayerDeath;
    }

    private void goDown()
    {
        if (!isActive || isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, -yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = true;
    }

    private void goUp()
    {
        if (!isActive || !isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = false;
    }

    private void goLeft()
    {
        if (!isActive) return;

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
        if (!isActive) return;

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

    // When the GameOver state is reached, the camera should no longer be able to move.
    private void OnPlayerDeath() => isActive = false;

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed  -= goDown;
        GameController.CurrentGameController.InputController.onSpacebarLeft     -= goUp;
        GameController.CurrentGameController.InputController.onLeftPressed      -= goLeft;
        GameController.CurrentGameController.InputController.onRightPressed     -= goRight;
        GameController.CurrentGameController.onPlayerDeath                      -= OnPlayerDeath;
    }
}
