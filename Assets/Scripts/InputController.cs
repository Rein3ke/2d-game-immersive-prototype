using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpacePressed();
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            SpaceLeft();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            LeftPressed();
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            RightPressed();
        }
        if (Input.GetMouseButtonDown(0))
        {
            LeftMousePressed();
        }
    }

    public event Action onSpacebarPressed;
    public void SpacePressed()
    {
        if (onSpacebarPressed != null)
        {
            onSpacebarPressed();
        }
    }

    public event Action onSpacebarLeft;
    public void SpaceLeft()
    {
        if (onSpacebarLeft != null)
        {
            onSpacebarLeft();
        }
    }

    public event Action onLeftPressed;
    public void LeftPressed()
    {
        if (onLeftPressed != null)
        {
            onLeftPressed();
        }
    }

    public event Action onRightPressed;
    public void RightPressed()
    {
        if (onRightPressed != null)
        {
            onRightPressed();
        }
    }

    public event Action onLeftMousePressed;
    public void LeftMousePressed()
    {
        if (onLeftMousePressed != null)
        {
            onLeftMousePressed();
        }
    }
}
