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
    private GameObject gameOverPanel;
    [SerializeField]
    private Button gameOverButton;

    private Canvas canvas;
    private Camera mainCamera;

    private void Start()
    {
        gameOverPanel.SetActive(false);

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

        gameOverButton.onClick.AddListener(OnGameOverButtonClicked);

        // Subscribe to events
        GameController.CurrentGameController.onPlayerHealthChange += OnPlayerHealthChange;
        GameController.CurrentGameController.onPlayerDeath += toggleGameOverPanel;
        GunController.Instance.onAmmunitionChange += OnAmmunitionChange;

        // Set values from player settings
        OnAmmunitionChange();
    }

    private void OnAmmunitionChange()
    {
        SetTextfieldText(playerAmmoText, "Ammo: " + playerSettings.playerAmmunition + "/" + playerSettings.playerMaxAmmunition);
    }

    private void OnGameOverButtonClicked()
    {
        GameController.CurrentGameController.loadMenu();
    }

    private void toggleGameOverPanel()
    {
        gameOverPanel.SetActive(!gameOverPanel.activeSelf);
    }

    private void OnPlayerHealthChange()
    {
        SetTextfieldText(playerHealthText, "Life: " + Mathf.Clamp(playerSettings.playerHealth, 0.0f, playerSettings.playerMaxHealth) + " HP");
    }

    private void SetTextfieldText(Text element, string content)
    {
        element.text = content;
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.onPlayerHealthChange -= OnPlayerHealthChange;
        GameController.CurrentGameController.onPlayerDeath -= toggleGameOverPanel;
        GunController.Instance.onAmmunitionChange -= OnAmmunitionChange;
    }
}
