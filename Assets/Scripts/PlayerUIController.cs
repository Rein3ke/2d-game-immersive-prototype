using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    private Canvas canvas;
    private Camera mainCamera;
    [SerializeField]
    private Text playerHealthText;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Button gameOverButton;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
        gameOverPanel.SetActive(false);

        if (mainCamera != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = mainCamera;
        }

        gameOverButton.onClick.AddListener(GameOverButtonClicked);

        // Subscribe to events
        GameController.CurrentGameController.onPlayerHealthChange += setPlayerHealthText;
        GameController.CurrentGameController.onPlayerDeath += toggleGameOverPanel;
    }

    private void GameOverButtonClicked()
    {
        GameController.CurrentGameController.loadMenu();
    }

    private void toggleGameOverPanel()
    {
        gameOverPanel.SetActive(!gameOverPanel.activeSelf);
    }

    private void setPlayerHealthText()
    {
        playerHealthText.text = "Life: " + GameController.CurrentGameController.PlayerHealth + " HP";
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.onPlayerHealthChange -= setPlayerHealthText;
        GameController.CurrentGameController.onPlayerDeath -= toggleGameOverPanel;
    }
}
