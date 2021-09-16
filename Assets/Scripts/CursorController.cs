using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorTexture;
    [SerializeField]
    private Texture2D cursorHitTexture;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;

    private Texture2D _currentTexture;

    private Vector2 hotSpot;

    private void Start()
    {
        _currentTexture = cursorTexture;
        hotSpot = new Vector2(_currentTexture.width / 2, _currentTexture.height / 2);

        Level.i.onScoreChange += OnScoreChange;
    }

    private void OnScoreChange()
    {
        StartCoroutine(SwitchTexture(.2f));
    }

    private IEnumerator SwitchTexture(float duration)
    {
        _currentTexture = cursorHitTexture;
        yield return new WaitForSeconds(duration);
        _currentTexture = cursorTexture;
        yield return null;
    }

    private void Update()
    {
        if (Application.isFocused)
        {
            Cursor.SetCursor(_currentTexture, hotSpot, cursorMode);
        }
    }



    private void OnDestroy()
    {
        Level.i.onScoreChange -= OnScoreChange;
    }
}
