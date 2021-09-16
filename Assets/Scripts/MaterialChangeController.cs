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
                StartCoroutine(ChangeValue(GameAssets.i.blur, "_BlurAmount", GameAssets.i.blur.GetFloat("_BlurAmount"), blurAmountDefault * 2f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_background, "_BlurAmount", GameAssets.i.blur_background.GetFloat("_BlurAmount"), blurAmountBackground * 2f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.universalBackgroundImage, "_BlurAmount", GameAssets.i.universalBackgroundImage.GetFloat("_BlurAmount"), blurAmountBackgroundImage * 2f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_foreground, "_BlurAmount", GameAssets.i.blur_foreground.GetFloat("_BlurAmount"), blurAmountForeground / 4f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_interactables, "_BlurAmount", GameAssets.i.blur_interactables.GetFloat("_BlurAmount"), blurAmountInteractables, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_enemies, "_BlurAmount", GameAssets.i.blur_enemies.GetFloat("_BlurAmount"), blurAmountEnemies, .2f));

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
                StartCoroutine(ChangeValue(GameAssets.i.blur, "_Saturation", GameAssets.i.blur.GetFloat("_Saturation"), 0.3f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_background, "_Saturation", GameAssets.i.blur_background.GetFloat("_Saturation"), 0.3f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_foreground, "_Saturation", GameAssets.i.blur_foreground.GetFloat("_Saturation"), 0.3f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_interactables, "_Saturation", GameAssets.i.blur_interactables.GetFloat("_Saturation"), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_enemies, "_Saturation", GameAssets.i.blur_enemies.GetFloat("_Saturation"), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.groundMaterial, "_Saturation", GameAssets.i.groundMaterial.GetFloat("_Saturation"), 0.3f, .2f));
                
                break;
        }
    }

    private void OnRightMouseUp()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.VISION:
                StartCoroutine(ChangeValue(GameAssets.i.blur, "_Saturation", GameAssets.i.blur.GetFloat("_Saturation"), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_background, "_Saturation", GameAssets.i.blur_background.GetFloat("_Saturation"), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_foreground, "_Saturation", GameAssets.i.blur_foreground.GetFloat("_Saturation"), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_interactables, "_Saturation", GameAssets.i.blur_interactables.GetFloat("_Saturation"), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.blur_enemies, "_Saturation", GameAssets.i.blur_enemies.GetFloat("_Saturation"), 1.0f, .2f));
                StartCoroutine(ChangeValue(GameAssets.i.groundMaterial, "_Saturation", GameAssets.i.groundMaterial.GetFloat("_Saturation"), 1.0f, .2f));

                break;
        }
    }

    private IEnumerator ChangeValue(Material material, string method, float v_start, float v_end, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            material.SetFloat(method, Mathf.Lerp(v_start, v_end, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        material.SetFloat(method, v_end);
        yield return null;
    }

    private void OnDistroy()
    {
        _inputController.onSpacebarDown -= OnSpacebarPressed;
        _inputController.onSpacebarUp -= OnSpacebarLeft;
        _inputController.onRightMouseDown -= OnRightMouseDown;
        _inputController.onRightMouseUp -= OnRightMouseUp;

        Level.i.onStateChange -= OnStateChange;
    }
}
