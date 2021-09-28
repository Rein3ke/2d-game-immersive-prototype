using System.Collections;
using Assets;
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
        InputController.onSpaceDown += InputController_OnSpaceDown;
        InputController.onSpaceUp += InputController_OnSpaceUp;
        InputController.onRightMouseDown += InputController_OnRightMouseDown;
        InputController.onRightMouseUp += InputController_OnRightMouseUp;
        Level.I.onStateChange += Level_OnStateChange;
    }

    /// <summary>
    /// Used to configure the materials stored under GameAssets. The configuration is defined by the current state.
    /// </summary>
    private void Level_OnStateChange()
    {
        // Reset the values of all materials to their default values
        ResetMaterialsInScene();
        
        // Set the correct values depending on the state
        switch (GameSettings.state)
        {
            case GameController.State.BLUR:
                GameAssets.I.universalDefault.SetFloat(BlurAmount, blurAmountDefault);
                GameAssets.I.universalBackground.SetFloat(BlurAmount, blurAmountBackground);
                GameAssets.I.universalBackgroundImage.SetFloat(BlurAmount, blurAmountBackgroundImage);
                GameAssets.I.universalForeground.SetFloat(BlurAmount, blurAmountForeground);
                GameAssets.I.universalInteractables.SetFloat(BlurAmount, 0.0f);
                GameAssets.I.universalEnemies.SetFloat(BlurAmount, 0.0f);
                break;
            case GameController.State.DOODLE:
                GameAssets.I.universalDefault.SetInt(DoodleEffect, 0);
                GameAssets.I.universalBackground.SetInt(DoodleEffect, 0);
                GameAssets.I.universalForeground.SetInt(DoodleEffect, 0);
                GameAssets.I.universalInteractables.SetInt(DoodleEffect, 1);
                GameAssets.I.universalEnemies.SetInt(DoodleEffect, 1);
                break;
        }
    }

    /// <summary>
    /// Resets all materials that may have been modified during runtime.
    /// </summary>
    private static void ResetMaterialsInScene()
    {
        // Blur Reset
        GameAssets.I.universalDefault.SetFloat(BlurAmount, 0f);
        GameAssets.I.universalBackground.SetFloat(BlurAmount, 0f);
        GameAssets.I.universalBackgroundImage.SetFloat(BlurAmount, 0f);
        GameAssets.I.universalForeground.SetFloat(BlurAmount, 0f);
        GameAssets.I.universalInteractables.SetFloat(BlurAmount, 0f);
        GameAssets.I.universalEnemies.SetFloat(BlurAmount, 0f);
        // Saturation Reset
        GameAssets.I.universalDefault.SetFloat(Saturation, 1f);
        GameAssets.I.universalBackground.SetFloat(Saturation, 1f);
        GameAssets.I.universalForeground.SetFloat(Saturation, 1f);
        GameAssets.I.universalInteractables.SetFloat(Saturation, 1f);
        GameAssets.I.universalEnemies.SetFloat(Saturation, 1f);
        GameAssets.I.universal3dDefault.SetFloat(Saturation, 1f);
        // Color Reset
        GameAssets.I.universalDefault.SetColor(MaterialColor, Color.white);
        GameAssets.I.universalBackground.SetColor(MaterialColor, Color.white);
        GameAssets.I.universalForeground.SetColor(MaterialColor, Color.white);
        GameAssets.I.universalInteractables.SetColor(MaterialColor, Color.white);
        GameAssets.I.universalEnemies.SetColor(MaterialColor, Color.white);
        // Doodle Effect Reset
        GameAssets.I.universalDefault.SetInt(DoodleEffect, 0);
        GameAssets.I.universalBackground.SetInt(DoodleEffect, 0);
        GameAssets.I.universalForeground.SetInt(DoodleEffect, 0);
        GameAssets.I.universalInteractables.SetInt(DoodleEffect, 0);
        GameAssets.I.universalEnemies.SetInt(DoodleEffect, 0);
    }

    /// <summary>
    /// Handles the space bar event. Depending on the state, new values are set when the space bar is pressed.
    /// </summary>
    private void InputController_OnSpaceDown()
    {
        if (!Level.I.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.BLUR:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalDefault, "_BlurAmount", GameAssets.I.universalDefault.GetFloat(BlurAmount), blurAmountDefault * 2f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalBackground, "_BlurAmount", GameAssets.I.universalBackground.GetFloat(BlurAmount), blurAmountBackground * 2f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalBackgroundImage, "_BlurAmount", GameAssets.I.universalBackgroundImage.GetFloat(BlurAmount), blurAmountBackgroundImage * 2f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalForeground, "_BlurAmount", GameAssets.I.universalForeground.GetFloat(BlurAmount), blurAmountForeground / 4f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalInteractables, "_BlurAmount", GameAssets.I.universalInteractables.GetFloat(BlurAmount), blurAmountInteractables, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalEnemies, "_BlurAmount", GameAssets.I.universalEnemies.GetFloat(BlurAmount), blurAmountEnemies, .2f));

                break;
        }
    }
    
    /// <summary>
    /// Handles the space bar event. When leaving the space bar, the values of the materials are reset, depending on the state.
    /// </summary>
    private void InputController_OnSpaceUp()
    {
        if (!Level.I.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.BLUR:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalDefault, "_BlurAmount", GameAssets.I.universalDefault.GetFloat("_BlurAmount"), blurAmountDefault, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalBackground, "_BlurAmount", GameAssets.I.universalBackground.GetFloat("_BlurAmount"), blurAmountBackground, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalBackgroundImage, "_BlurAmount", GameAssets.I.universalBackgroundImage.GetFloat("_BlurAmount"), blurAmountBackgroundImage, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalForeground, "_BlurAmount", GameAssets.I.universalForeground.GetFloat("_BlurAmount"), blurAmountForeground, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalInteractables, "_BlurAmount", GameAssets.I.universalInteractables.GetFloat("_BlurAmount"), 0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalEnemies, "_BlurAmount", GameAssets.I.universalEnemies.GetFloat("_BlurAmount"), 0f, .2f));

                break;
        }
    }

    /// <summary>
    /// Handles the right mouse button event. Here the values of the materials are set, depending on the state.
    /// </summary>
    private void InputController_OnRightMouseDown()
    {
        if (!Level.I.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.VISION:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalDefault, "_Saturation", GameAssets.I.universalDefault.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalBackground, "_Saturation", GameAssets.I.universalBackground.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalForeground, "_Saturation", GameAssets.I.universalForeground.GetFloat(Saturation), 0.3f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalInteractables, "_Saturation", GameAssets.I.universalInteractables.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalEnemies, "_Saturation", GameAssets.I.universalEnemies.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universal3dDefault, "_Saturation", GameAssets.I.universal3dDefault.GetFloat(Saturation), 0.3f, .2f));
                
                break;
        }
    }

    /// <summary>
    /// Handles the right mouse button event. Here the values of the materials are reset, depending on the state.
    /// </summary>
    private void InputController_OnRightMouseUp()
    {
        if (!Level.I.IsGameRunning) return;

        switch (GameSettings.state)
        {
            case GameController.State.VISION:
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalDefault, "_Saturation", GameAssets.I.universalDefault.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalBackground, "_Saturation", GameAssets.I.universalBackground.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalForeground, "_Saturation", GameAssets.I.universalForeground.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalInteractables, "_Saturation", GameAssets.I.universalInteractables.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universalEnemies, "_Saturation", GameAssets.I.universalEnemies.GetFloat(Saturation), 1.0f, .2f));
                StartCoroutine(SmoothMaterialValueChange(GameAssets.I.universal3dDefault, "_Saturation", GameAssets.I.universal3dDefault.GetFloat(Saturation), 1.0f, .2f));

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
        InputController.onSpaceDown -= InputController_OnSpaceDown;
        InputController.onSpaceUp -= InputController_OnSpaceUp;
        InputController.onRightMouseDown -= InputController_OnRightMouseDown;
        InputController.onRightMouseUp -= InputController_OnRightMouseUp;

        Level.I.onStateChange -= Level_OnStateChange;
    }
}
