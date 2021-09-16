using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangeController : MonoBehaviour
{
    [SerializeField]
    private float blurAmountDefault;
    [SerializeField]
    private float blurAmountForeground;
    [SerializeField]
    private float blurAmountBackground;
    [SerializeField]
    private float blurAmountBackgroundImage;
    [SerializeField]
    private float blurAmountEnemies;
    [SerializeField]
    private float blurAmountInteractables;

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
                GameAssets.i.blur.SetFloat("_BlurAmount", blurAmountDefault);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", blurAmountBackground);
                GameAssets.i.universalBackgroundImage.SetFloat("_BlurAmount", blurAmountBackgroundImage);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", blurAmountForeground);
                GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0.0f);
                GameAssets.i.blur_enemies.SetFloat("_BlurAmount", 0.0f);
                break;
            case GameController.State.DOODLE:
                GameAssets.i.blur.SetInt("_DoodleEffect", 0);
                GameAssets.i.blur_background.SetInt("_DoodleEffect", 0);
                GameAssets.i.blur_foreground.SetInt("_DoodleEffect", 0);
                GameAssets.i.blur_interactables.SetInt("_DoodleEffect", 1);
                GameAssets.i.blur_enemies.SetInt("_DoodleEffect", 1);
                break;
        }
    }

    private void ResetMaterialsInScene()
    {
        // Blur Reset
        GameAssets.i.blur.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_background.SetFloat("_BlurAmount", 0f);
        GameAssets.i.universalBackgroundImage.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_foreground.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0f);
        GameAssets.i.blur_enemies.SetFloat("_BlurAmount", 0f);
        // Saturation Reset
        GameAssets.i.blur.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_background.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_foreground.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_interactables.SetFloat("_Saturation", 1f);
        GameAssets.i.blur_enemies.SetFloat("_Saturation", 1f);
        GameAssets.i.groundMaterial.SetFloat("_Saturation", 1f);
        // Color Reset
        GameAssets.i.blur.SetColor("_Color", Color.white);
        GameAssets.i.blur_background.SetColor("_Color", Color.white);
        GameAssets.i.blur_foreground.SetColor("_Color", Color.white);
        GameAssets.i.blur_interactables.SetColor("_Color", Color.white);
        GameAssets.i.blur_enemies.SetColor("_Color", Color.white);
        // Doodle Effect Reset
        GameAssets.i.blur.SetInt("_DoodleEffect", 0);
        GameAssets.i.blur_background.SetInt("_DoodleEffect", 0);
        GameAssets.i.blur_foreground.SetInt("_DoodleEffect", 0);
        GameAssets.i.blur_interactables.SetInt("_DoodleEffect", 0);
        GameAssets.i.blur_enemies.SetInt("_DoodleEffect", 0);
    }

    private void OnSpacebarPressed()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                GameAssets.i.blur.SetFloat("_BlurAmount", blurAmountDefault * 2f);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", blurAmountBackground * 2f);
                GameAssets.i.universalBackgroundImage.SetFloat("_BlurAmount", blurAmountBackgroundImage * 2f);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", blurAmountForeground / 4);
                GameAssets.i.blur_interactables.SetFloat("_BlurAmount", blurAmountInteractables);
                GameAssets.i.blur_enemies.SetFloat("_BlurAmount", blurAmountEnemies);
                break;
        }
    }

    private void OnSpacebarLeft()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                GameAssets.i.blur.SetFloat("_BlurAmount", blurAmountDefault);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", blurAmountBackground);
                GameAssets.i.universalBackgroundImage.SetFloat("_BlurAmount", blurAmountBackgroundImage);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", blurAmountForeground);
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
                GameAssets.i.blur.SetFloat("_Saturation", 0.3f);
                GameAssets.i.blur_background.SetFloat("_Saturation", 0.3f);
                GameAssets.i.blur_foreground.SetFloat("_Saturation", 0.3f);
                GameAssets.i.blur_interactables.SetFloat("_Saturation", 1.0f);
                GameAssets.i.blur_enemies.SetFloat("_Saturation", 1.0f);
                GameAssets.i.groundMaterial.SetFloat("_Saturation", 0.3f);
                break;
        }
    }

    private void OnRightMouseUp()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.VISION:
                GameAssets.i.blur.SetFloat("_Saturation", 1.0f);
                GameAssets.i.blur_background.SetFloat("_Saturation", 1.0f);
                GameAssets.i.blur_foreground.SetFloat("_Saturation", 1.0f);
                GameAssets.i.blur_interactables.SetFloat("_Saturation", 1.0f);
                GameAssets.i.blur_enemies.SetFloat("_Saturation", 1.0f);
                GameAssets.i.groundMaterial.SetFloat("_Saturation", 1.0f);
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
