using System;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public PlayerSettings PlayerSettings { get; set; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            KeyEscapeDown();
        }
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
    public event Action onKeyEscapeDown;
    private void KeyEscapeDown()
    {
        onKeyEscapeDown?.Invoke();
    }

    public event Action onSpacebarDown;
    public void SpaceDown()
    {
        PlayerSettings.SpacebarPressedCount++;
        onSpacebarDown?.Invoke();
    }

    public event Action onSpacebarUp;
    public void SpaceLeft()
    {
        onSpacebarUp?.Invoke();
    }

    public event Action onLeftDown;
    public void LeftDown()
    {
        onLeftDown?.Invoke();
    }

    public event Action onRightDown;
    public void RightDown()
    {
        onRightDown?.Invoke();
    }

    public event Action onLeftMouseDown;
    public void LeftMouseDown()
    {
        onLeftMouseDown?.Invoke();
    }

    public event Action onRightMouseDown;
    public void RightMouseDown()
    {
        PlayerSettings.RightClickedCount++;
        onRightMouseDown?.Invoke();
    }

    public event Action onRightMouseUp;
    public void RightMouseUp()
    {
        onRightMouseUp?.Invoke();
    }

    public event Action onKeyRDown;
    public void KeyRDown()
    {
        onKeyRDown?.Invoke();
    }
    #endregion
}
