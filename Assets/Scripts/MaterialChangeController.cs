using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeController : MonoBehaviour
{
    public GameSettings GameSettings
    {
        set => _gameSettings = value;
    }

    private GameSettings _gameSettings;
    private InputController _inputController;

    void Start()
    {
        _inputController = GameController.CurrentGameController.InputController;

        _inputController.onSpacebarDown += OnSpacebarPressed;
        _inputController.onSpacebarUp += OnSpacebarLeft;
        _inputController.onRightMouseDown += OnRightMouseDown;
        _inputController.onRightMouseUp += OnRightMouseUp;

        Level.i.onStateChange += OnStateChange;
    }

    private void OnStateChange()
    {
        ResetMaterialsInScene();
        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                GameAssets.i.blur.SetFloat("_BlurAmount", 0.07f);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", 0.2f);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", 0.1f);
                GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0.0f);
                GameAssets.i.blur_enemies.SetFloat("_BlurAmount", 0.0f);
                break;
        }
    }

    private void ResetMaterialsInScene()
    {
        // Blur Reset
        GameAssets.i.blur.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_background.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_foreground.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_enemies.SetFloat("_BlurAmount", 0f);
        // Saturation Reset
        GameAssets.i.blur.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_background.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_foreground.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_interactables.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_enemies.SetFloat("_Saturation", 1f);
        // Color Reset
        GameAssets.i.blur.SetColor("_Color", Color.white);
        GameAssets.i.blur_background.SetColor("_Color", Color.white);
        GameAssets.i.blur_foreground.SetColor("_Color", Color.white);
        GameAssets.i.blur_interactables.SetColor("_Color", Color.white);
        GameAssets.i.blur_enemies.SetColor("_Color", Color.white);
    }

    private void OnSpacebarPressed()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                GameAssets.i.blur.SetFloat("_BlurAmount", 0.3f);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", 0.4f);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", 0.01f);
                GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0.3f);
                GameAssets.i.blur_enemies.SetFloat("_BlurAmount", 0.3f);
                break;
        }
    }

    private void OnSpacebarLeft()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                GameAssets.i.blur.SetFloat("_BlurAmount", 0.07f);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", 0.2f);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", 0.1f);
                GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0.0f);
                GameAssets.i.blur_enemies.SetFloat("_BlurAmount", 0.0f);
                break;
        }
    }

    private void OnRightMouseDown()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.VISION:
                GameAssets.i.blur.SetColor("_Color", Color.gray);
                GameAssets.i.blur_background.SetColor("_Color", Color.gray);
                GameAssets.i.blur_foreground.SetColor("_Color", Color.gray);
                GameAssets.i.blur_interactables.SetColor("_Color", Color.green);
                GameAssets.i.blur_enemies.SetColor("_Color", Color.red);
                break;
        }
    }

    private void OnRightMouseUp()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.VISION:
                GameAssets.i.blur.SetColor("_Color", Color.white);
                GameAssets.i.blur_background.SetColor("_Color", Color.white);
                GameAssets.i.blur_foreground.SetColor("_Color", Color.white);
                GameAssets.i.blur_interactables.SetColor("_Color", Color.white);
                GameAssets.i.blur_enemies.SetColor("_Color", Color.white);
                break;
        }
    }

    private void OnDisable()
    {
        _inputController.onSpacebarDown -= OnSpacebarPressed;
        _inputController.onSpacebarUp -= OnSpacebarLeft;
        _inputController.onRightMouseDown -= OnRightMouseDown;
        _inputController.onRightMouseUp -= OnRightMouseUp;

        Level.i.onStateChange -= OnStateChange;
    }
}
