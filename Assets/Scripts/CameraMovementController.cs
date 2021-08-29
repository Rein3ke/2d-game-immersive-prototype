using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    [SerializeField]
    private float yOffset = 0.5f;
    [SerializeField]
    private float movementSpeed = 4.0f;

    private bool isDown = false;

    private void Start()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed += goDown;
        GameController.CurrentGameController.InputController.onSpacebarLeft += goUp;
        GameController.CurrentGameController.InputController.onLeftPressed += goLeft;
        GameController.CurrentGameController.InputController.onRightPressed += goRight;
    }

    private void goDown()
    {
        if (isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, -yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = true;
    }

    private void goUp()
    {
        if (!isDown) return;

        Vector3 currentPosition = transform.position;
        transform.position = Vector3.Lerp(currentPosition, new Vector3(currentPosition.x, yOffset, currentPosition.z), Mathf.SmoothStep(0.0f, 1.0f, Mathf.SmoothStep(0.0f, 1.0f, .5f)));
        isDown = false;
    }

    private void goLeft()
    {
        if (isDown)
        {
            transform.position += Vector3.left * (movementSpeed / 3) * Time.deltaTime;
        } else
        {
            transform.position += Vector3.left * movementSpeed * Time.deltaTime;
        }
    }

    private void goRight()
    {
        if (isDown)
        {
            transform.position += Vector3.right * (movementSpeed / 3) * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed  -= goDown;
        GameController.CurrentGameController.InputController.onSpacebarLeft     -= goUp;
        GameController.CurrentGameController.InputController.onLeftPressed      -= goLeft;
        GameController.CurrentGameController.InputController.onRightPressed     -= goRight;
    }
}
