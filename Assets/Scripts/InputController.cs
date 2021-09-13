using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpaceDown();
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            SpaceLeft();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            LeftDown();
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            RightDown();
        }
        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseDown();
        }
        if (Input.GetMouseButtonDown(1))
        {
            RightMouseDown();
        }
        if (Input.GetMouseButtonUp(1))
        {
            RightMouseUp();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            KeyRDown();
        }
    }

    #region Events
    public event Action onSpacebarDown;
    public void SpaceDown()
    {
        if (onSpacebarDown != null)
        {
            onSpacebarDown();
        }
    }

    public event Action onSpacebarUp;
    public void SpaceLeft()
    {
        if (onSpacebarUp != null)
        {
            onSpacebarUp();
        }
    }

    public event Action onLeftDown;
    public void LeftDown()
    {
        if (onLeftDown != null)
        {
            onLeftDown();
        }
    }

    public event Action onRightDown;
    public void RightDown()
    {
        if (onRightDown != null)
        {
            onRightDown();
        }
    }

    public event Action onLeftMouseDown;
    public void LeftMouseDown()
    {
        if (onLeftMouseDown != null)
        {
            onLeftMouseDown();
        }
    }

    public event Action onRightMouseDown;
    public void RightMouseDown()
    {
        if (onRightMouseDown != null)
        {
            onRightMouseDown();
        }
    }

    public event Action onRightMouseUp;
    public void RightMouseUp()
    {
        if (onRightMouseUp != null)
        {
            onRightMouseUp();
        }
    }

    public event Action onKeyRDown;
    public void KeyRDown()
    {
        if (onKeyRDown != null)
        {
            onKeyRDown();
        }
    }
    #endregion
}
