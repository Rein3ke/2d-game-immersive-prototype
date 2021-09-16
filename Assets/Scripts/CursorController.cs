using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorTexture;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;

    private Vector2 hotSpot;

    private void Start()
    {
        hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
    }

    private void Update()
    {
        if (Application.isFocused)
        {
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }
    }
}
