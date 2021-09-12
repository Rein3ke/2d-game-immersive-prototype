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

    void Start()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed += OnSpacebarPressed;
        GameController.CurrentGameController.InputController.onSpacebarLeft += OnSpacebarLeft;

        Level.i.onStateChange += OnStateChange;
    }

    private void OnStateChange()
    {
        ResetMaterialsInScene();
        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                GameObject[] decoration = GameObject.FindGameObjectsWithTag("Decoration");
                GameObject[] decorationForeground = GameObject.FindGameObjectsWithTag("Decoration_Foreground");
                GameObject[] decorationBackground = GameObject.FindGameObjectsWithTag("Decoration_Background");

                SetMaterialOnGameObjects(decoration, GameAssets.i.blur);
                SetMaterialOnGameObjects(decorationForeground, GameAssets.i.blur_foreground);
                SetMaterialOnGameObjects(decorationBackground, GameAssets.i.blur_background);

                GameAssets.i.blur.SetFloat("_BlurAmount", 0.07f);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", 0.2f);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", 0.1f);
                GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0.0f);

                break;
        }
    }

    private void SetMaterialOnGameObjects(GameObject[] gameObjects, Material material)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.GetComponent<SpriteRenderer>().material = material;
        }
    }

    private void ResetMaterialsInScene()
    {
        GameObject[] decoration = GameObject.FindGameObjectsWithTag("Decoration");
        GameObject[] decorationForeground = GameObject.FindGameObjectsWithTag("Decoration_Foreground");
        GameObject[] decorationBackground = GameObject.FindGameObjectsWithTag("Decoration_Background");

        SetMaterialOnGameObjects(decoration, GameAssets.i.defaultMaterial);
        SetMaterialOnGameObjects(decorationForeground, GameAssets.i.defaultMaterial);
        SetMaterialOnGameObjects(decorationBackground, GameAssets.i.defaultMaterial);
    }

    private void OnSpacebarPressed()
    {
        if (!Level.i.IsGameRunning) return;

        switch (_gameSettings.state)
        {
            case GameController.State.BLUR:
                foreach(EnemyController enemyController in FindObjectsOfType<EnemyController>())
                {
                    enemyController.SpriteRenderer.material = GameAssets.i.blur_interactables;
                }
                foreach (InteractableObjectController interactableObject in FindObjectsOfType<InteractableObjectController>())
                {
                    interactableObject.SpriteRenderer.material = GameAssets.i.blur_interactables;
                }

                GameAssets.i.blur.SetFloat("_BlurAmount", 0.3f);
                GameAssets.i.blur_background.SetFloat("_BlurAmount", 0.4f);
                GameAssets.i.blur_foreground.SetFloat("_BlurAmount", 0.01f);
                GameAssets.i.blur_interactables.SetFloat("_BlurAmount", 0.3f);
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
                break;
        }
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onSpacebarPressed -= OnSpacebarPressed;
        GameController.CurrentGameController.InputController.onSpacebarLeft -= OnSpacebarLeft;
        Level.i.onStateChange -= OnStateChange;
    }
}
