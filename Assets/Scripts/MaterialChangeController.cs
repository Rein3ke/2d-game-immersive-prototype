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
    // Property IDs
    private static readonly int BlurAmount = Shader.PropertyToID("_BlurAmount");
    private static readonly int DoodleEffect = Shader.PropertyToID("_DoodleEffect");
    private static readonly int Saturation = Shader.PropertyToID("_Saturation");
    private static readonly int MaterialColor = Shader.PropertyToID("_Color");

    private void Start()
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
                GameAssets.i.blur.SetFloat(BlurAmount, blurAmountDefault);
                GameAssets.i.blur_background.SetFloat(BlurAmount, blurAmountBackground);
                GameAssets.i.universalBackgroundImage.SetFloat(BlurAmount, blurAmountBackgroundImage);
                GameAssets.i.blur_foreground.SetFloat(BlurAmount, blurAmountForeground);
                GameAssets.i.blur_interactables.SetFloat(BlurAmount, 0.0f);
                GameAssets.i.blur_enemies.SetFloat(BlurAmount, 0.0f);
                break;
            case GameController.State.DOODLE:
                GameAssets.i.blur.SetInt(DoodleEffect, 0);
                GameAssets.i.blur_background.SetInt(DoodleEffect, 0);
                GameAssets.i.blur_foreground.SetInt(DoodleEffect, 0);
                GameAssets.i.blur_interactables.SetInt(DoodleEffect, 1);
                GameAssets.i.blur_enemies.SetInt(DoodleEffect, 1);
                break;
        }
    }

    private static void ResetMaterialsInScene()
    {
        // Blur Reset
        GameAssets.i.blur.SetFloat(BlurAmount, 0f);
        GameAssets.i.blur_background.SetFloat(BlurAmount, 0f);
        GameAssets.i.universalBackgroundImage.SetFloat(BlurAmount, 0f);
        GameAssets.i.blur_foreground.SetFloat(BlurAmount, 0f);
        GameAssets.i.blur_interactables.SetFloat(BlurAmount, 0f);
        GameAssets.i.blur_enemies.SetFloat(BlurAmount, 0f);
        // Saturation Reset
        GameAssets.i.blur.SetFloat(Saturation, 1f);
        GameAssets.i.blur_background.SetFloat(Saturation, 1f);
        GameAssets.i.blur_foreground.SetFloat(Saturation, 1f);
        GameAssets.i.blur_interactables.SetFloat(Saturation, 1f);
        GameAssets.i.blur_enemies.SetFloat(Saturation, 1f);
        GameAssets.i.groundMaterial.SetFloat(Saturation, 1f);
        // Color Reset
        GameAssets.i.blur.SetColor(MaterialColor, Color.white);
        GameAssets.i.blur_background.SetColor(MaterialColor, Color.white);
        GameAssets.i.blur_foreground.SetColor(MaterialColor, Color.white);
        GameAssets.i.blur_interactables.SetColor(MaterialColor, Color.white);
        GameAssets.i.blur_enemies.SetColor(MaterialColor, Color.white);
        // Doodle Effect Reset
        GameAssets.i.blur.SetInt(DoodleEffect, 0);
        GameAssets.i.blur_background.SetInt(DoodleEffect, 0);
        GameAssets.i.blur_foreground.SetInt(DoodleEffect, 0);
        GameAssets.i.blur_interactables.SetInt(DoodleEffect, 0);
        GameAssets.i.blur_enemies.SetInt(DoodleEffect, 0);
    }

    private void OnSpacebarPressed()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                StartCoroutine(ChangeValue(GameAssets.i.blur, "_BlurAmount", GameAssets.i.blur.GetFloat(BlurAmount), blurAmountDefault * 2f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_background, "_BlurAmount", GameAssets.i.blur_background.GetFloat(BlurAmount), blurAmountBackground * 2f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.universalBackgroundImage, "_BlurAmount", GameAssets.i.universalBackgroundImage.GetFloat(BlurAmount), blurAmountBackgroundImage * 2f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_foreground, "_BlurAmount", GameAssets.i.blur_foreground.GetFloat(BlurAmount), blurAmountForeground / 4f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_interactables, "_BlurAmount", GameAssets.i.blur_interactables.GetFloat(BlurAmount), blurAmountInteractables, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_enemies, "_BlurAmount", GameAssets.i.blur_enemies.GetFloat(BlurAmount), blurAmountEnemies, .2f));

                break;
        }
    }

    private void OnSpacebarLeft()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                StartCoroutine(ChangeValue(GameAssets.i.blur, "_BlurAmount", GameAssets.i.blur.GetFloat("_BlurAmount"), blurAmountDefault, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_background, "_BlurAmount", GameAssets.i.blur_background.GetFloat("_BlurAmount"), blurAmountBackground, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.universalBackgroundImage, "_BlurAmount", GameAssets.i.universalBackgroundImage.GetFloat("_BlurAmount"), blurAmountBackgroundImage, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_foreground, "_BlurAmount", GameAssets.i.blur_foreground.GetFloat("_BlurAmount"), blurAmountForeground, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_interactables, "_BlurAmount", GameAssets.i.blur_interactables.GetFloat("_BlurAmount"), 0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_enemies, "_BlurAmount", GameAssets.i.blur_enemies.GetFloat("_BlurAmount"), 0f, .2f));

                break;
        }
    }

    private void OnRightMouseDown()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.VISION:
                StartCoroutine(ChangeValue(GameAssets.i.blur, "_Saturation", GameAssets.i.blur.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_background, "_Saturation", GameAssets.i.blur_background.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_foreground, "_Saturation", GameAssets.i.blur_foreground.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_interactables, "_Saturation", GameAssets.i.blur_interactables.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_enemies, "_Saturation", GameAssets.i.blur_enemies.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.groundMaterial, "_Saturation", GameAssets.i.groundMaterial.GetFloat(Saturation), 0.3f, .2f));
                
                break;
        }
    }

    private void OnRightMouseUp()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.VISION:
                StartCoroutine(ChangeValue(GameAssets.i.blur, "_Saturation", GameAssets.i.blur.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_background, "_Saturation", GameAssets.i.blur_background.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_foreground, "_Saturation", GameAssets.i.blur_foreground.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_interactables, "_Saturation", GameAssets.i.blur_interactables.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_enemies, "_Saturation", GameAssets.i.blur_enemies.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.groundMaterial, "_Saturation", GameAssets.i.groundMaterial.GetFloat(Saturation), 1.0f, .2f));

                break;
        }
    }

    private static IEnumerator ChangeValue(Material material, string method, float vStart, float vEnd, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            material.SetFloat(method, Mathf.Lerp(vStart, vEnd, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        material.SetFloat(method, vEnd);
        yield return null;
    }

    private void OnDestroy()
    {
        _inputController.onSpacebarDown -= OnSpacebarPressed;
        _inputController.onSpacebarUp -= OnSpacebarLeft;
        _inputController.onRightMouseDown -= OnRightMouseDown;
        _inputController.onRightMouseUp -= OnRightMouseUp;

        Level.i.onStateChange -= OnStateChange;
    }
}
