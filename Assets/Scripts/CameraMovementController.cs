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

    private Vector3 cameraPosition;
    public Vector3 CameraPosition
    {
        get => cameraPosition;
        set
        {
            transform.position = value;
        }
    }

    [SerializeField]
    private float yOffset = 0.5f;
    [SerializeField]
    private float movementSpeed = 4.0f;

    private bool isDown = false;
    private bool isGameOver = false;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else Debug.LogError("Too many camera controllers!");
    }

    private void Start()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed += goDown;
        GameController.CurrentGameController.InputController.onSpacebarLeft += goUp;
        GameController.CurrentGameController.InputController.onLeftPressed += goLeft;
        GameController.CurrentGameController.InputController.onRightPressed += goRight;
        GameController.CurrentGameController.onPlayerDeath += setIsGameOver;
    }

    private void goDown()
    {
        if (isGameOver || isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, -yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = true;
    }

    private void goUp()
    {
        if (isGameOver || !isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = false;
    }

    private void goLeft()
    {
        if (isGameOver) return;

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
        if (isGameOver) return;

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

    private void setIsGameOver() => isGameOver = true;

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed  -= goDown;
        GameController.CurrentGameController.InputController.onSpacebarLeft     -= goUp;
        GameController.CurrentGameController.InputController.onLeftPressed      -= goLeft;
        GameController.CurrentGameController.InputController.onRightPressed     -= goRight;
        GameController.CurrentGameController.onPlayerDeath                      -= setIsGameOver;
    }
}
