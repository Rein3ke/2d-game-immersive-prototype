using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField]
    private PlayerSettings playerSettings;
    [SerializeField]
    private Text playerHealthText;
    [SerializeField]
    private Text playerAmmoText;
    [SerializeField]
    private Text playerScoreText;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject gameWonPanel;

    private Canvas canvas;
    private Camera mainCamera;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        gameWonPanel.SetActive(false);

        // Bind UI to main camera
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = mainCamera;
        } else
        {
            Debug.LogError("Error: No camera was found!");
        }

        // Subscribe to events
        GameController.CurrentGameController.onPlayerHealthChange += OnPlayerHealthChange;
        GameController.CurrentGameController.onScoreChange += OnScoreChange;
        GameController.CurrentGameController.onGameEnd += OnGameEnd;
        GameController.CurrentGameController.onGameWon += OnGameWon;
        GunController.Instance.onAmmunitionChange += OnAmmunitionChange;

        // Set values from player settings
        OnAmmunitionChange();
        OnPlayerHealthChange();
    }

    private void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    private void SetTextfieldText(Text element, string content)
    {
        element.text = content;
    }

    #region Event Handling
    public void LoadMenu()
    {
        GameController.CurrentGameController.loadMenu();
    }

    private void OnGameWon()
    {
        TogglePanel(gameWonPanel);
    }

    private void OnGameEnd()
    {
        TogglePanel(gameOverPanel);
    }

    private void OnAmmunitionChange()
    {
        SetTextfieldText(playerAmmoText, "Ammo: " + playerSettings.playerAmmunition + "/" + playerSettings.playerMaxAmmunition);
    }

    private void OnPlayerHealthChange()
    {
        SetTextfieldText(playerHealthText, "Life: " + Mathf.Clamp(playerSettings.playerHealth, 0.0f, playerSettings.playerMaxHealth) + " HP");
    }

    private void OnScoreChange()
    {
        SetTextfieldText(playerScoreText, "Score: " + playerSettings.score);
    }
    #endregion


    private void OnDisable()
    {
        GameController.CurrentGameController.onPlayerHealthChange -= OnPlayerHealthChange;
        GameController.CurrentGameController.onScoreChange -= OnScoreChange;
        GameController.CurrentGameController.onGameEnd -= OnGameEnd;
        GameController.CurrentGameController.onGameWon -= OnGameWon;
        GunController.Instance.onAmmunitionChange -= OnAmmunitionChange;
    }
}
