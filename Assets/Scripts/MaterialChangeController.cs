using System.Collections;
using UnityEngine;

/// <summary>
/// A controller used for exchanging materials stored under GameAssets.
/// </summary>
public class MaterialChangeController : MonoBehaviour
{
    [Header("Level of Blur")]
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

    // Properties
    public GameSettings GameSettings { get; set; }
    private InputController InputController { get; set; }
    
    // Property IDs
    private static readonly int BlurAmount = Shader.PropertyToID("_BlurAmount");
    private static readonly int DoodleEffect = Shader.PropertyToID("_DoodleEffect");
    private static readonly int Saturation = Shader.PropertyToID("_Saturation");
    private static readonly int MaterialColor = Shader.PropertyToID("_Color");

    private void Start()
    {
        // Get input controller reference
        InputController = GameController.Instance.InputController;

        // Set events
        InputController.onSpacebarDown += OnSpacebarPressed;
        InputController.onSpacebarUp += OnSpacebarLeft;
        InputController.onRightMouseDown += OnRightMouseDown;
        InputController.onRightMouseUp += OnRightMouseUp;
        Level.i.onStateChange += OnStateChange;
    }

    /// <summary>
    /// Used to configure the materials stored under GameAssets. The configuration is defined by the current state.
    /// </summary>
    private void OnStateChange()
    {
        // Reset the values of all materials to their default values
        ResetMaterialsInScene();
        
        // Set the correct values depending on the state
        switch (GameSettings.state)
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

    /// <summary>
    /// Resets all materials that may have been modified during runtime.
    /// </summary>
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

    /// <summary>
    /// Handles the space bar event. Depending on the state, new values are set when the space bar is pressed.
    /// </summary>
    private void OnSpacebarPressed()
    {
        if (!Level.i.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.BLUR:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur, "_BlurAmount", GameAssets.i.blur.GetFloat(BlurAmount), blurAmountDefault * 2f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_background, "_BlurAmount", GameAssets.i.blur_background.GetFloat(BlurAmount), blurAmountBackground * 2f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.universalBackgroundImage, "_BlurAmount", GameAssets.i.universalBackgroundImage.GetFloat(BlurAmount), blurAmountBackgroundImage * 2f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_foreground, "_BlurAmount", GameAssets.i.blur_foreground.GetFloat(BlurAmount), blurAmountForeground / 4f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_interactables, "_BlurAmount", GameAssets.i.blur_interactables.GetFloat(BlurAmount), blurAmountInteractables, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_enemies, "_BlurAmount", GameAssets.i.blur_enemies.GetFloat(BlurAmount), blurAmountEnemies, .2f));

                break;
        }
    }
    
    /// <summary>
    /// Handles the space bar event. When leaving the space bar, the values of the materials are reset, depending on the state.
    /// </summary>
    private void OnSpacebarLeft()
    {
        if (!Level.i.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.BLUR:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur, "_BlurAmount", GameAssets.i.blur.GetFloat("_BlurAmount"), blurAmountDefault, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_background, "_BlurAmount", GameAssets.i.blur_background.GetFloat("_BlurAmount"), blurAmountBackground, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.universalBackgroundImage, "_BlurAmount", GameAssets.i.universalBackgroundImage.GetFloat("_BlurAmount"), blurAmountBackgroundImage, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_foreground, "_BlurAmount", GameAssets.i.blur_foreground.GetFloat("_BlurAmount"), blurAmountForeground, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_interactables, "_BlurAmount", GameAssets.i.blur_interactables.GetFloat("_BlurAmount"), 0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_enemies, "_BlurAmount", GameAssets.i.blur_enemies.GetFloat("_BlurAmount"), 0f, .2f));

                break;
        }
    }

    /// <summary>
    /// Handles the right mouse button event. Here the values of the materials are set, depending on the state.
    /// </summary>
    private void OnRightMouseDown()
    {
        if (!Level.i.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.VISION:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur, "_Saturation", GameAssets.i.blur.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_background, "_Saturation", GameAssets.i.blur_background.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_foreground, "_Saturation", GameAssets.i.blur_foreground.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_interactables, "_Saturation", GameAssets.i.blur_interactables.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_enemies, "_Saturation", GameAssets.i.blur_enemies.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.groundMaterial, "_Saturation", GameAssets.i.groundMaterial.GetFloat(Saturation), 0.3f, .2f));
                
                break;
        }
    }

    /// <summary>
    /// Handles the right mouse button event. Here the values of the materials are reset, depending on the state.
    /// </summary>
    private void OnRightMouseUp()
    {
        if (!Level.i.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.VISION:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur, "_Saturation", GameAssets.i.blur.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_background, "_Saturation", GameAssets.i.blur_background.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_foreground, "_Saturation", GameAssets.i.blur_foreground.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_interactables, "_Saturation", GameAssets.i.blur_interactables.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.blur_enemies, "_Saturation", GameAssets.i.blur_enemies.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.i.groundMaterial, "_Saturation", GameAssets.i.groundMaterial.GetFloat(Saturation), 1.0f, .2f));

                break;
        }
    }

    /// <summary>
    /// Coroutine: Used for smooth transition between two values. Accepts a material, a shader reference id, a start and end value and a duration.
    /// </summary>
    /// <param name="material">Target material</param>
    /// <param name="shaderReferenceId">Shader reference id (string)</param>
    /// <param name="vStart"> Start value</param>
    /// <param name="vEnd">End value</param>
    /// <param name="duration">Duration in seconds</param>
    /// <returns>Nothing</returns>
    private static IEnumerator SmoothMaterialValueChange(Material material, string shaderReferenceId, float vStart, float vEnd, float duration)
    {
        float elapsed = 0.0f;
        
        while (elapsed < duration)
        {
            material.SetFloat(shaderReferenceId, Mathf.Lerp(vStart, vEnd, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        material.SetFloat(shaderReferenceId, vEnd);
        
        yield return null;
    }

    /// <summary>
    /// Unsubscribe from all events.
    /// </summary>
    private void OnDestroy()
    {
        InputController.onSpacebarDown -= OnSpacebarPressed;
        InputController.onSpacebarUp -= OnSpacebarLeft;
        InputController.onRightMouseDown -= OnRightMouseDown;
        InputController.onRightMouseUp -= OnRightMouseUp;

        Level.i.onStateChange -= OnStateChange;
    }
}
