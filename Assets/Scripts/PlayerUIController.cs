using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField]
    private Text playerHealthText;
    [SerializeField]
    private Text playerAmmoText;
    [SerializeField]
    private Text playerScoreText;
    [SerializeField]
    private Text activeFeatureText;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject gameWonPanel;

    private Canvas canvas;
    private Camera mainCamera;

    public PlayerSettings PlayerSettings
    {
        set => _playerSettings = value;
    }
    private PlayerSettings _playerSettings;

    public GameSettings GameSettings
    {
        set => _gameSettings = value;
    }
    private GameSettings _gameSettings;

    private void Start()
    {
        // Bind UI to main camera
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = mainCamera;
        }
        else
        {
            Debug.LogError("Error: No camera was found!");
        }

        // Subscribe to events
        Level.i.onPlayerHealthChange += OnPlayerHealthChange;
        Level.i.onScoreChange += OnScoreChange;
        Level.i.onGameLost += OnGameLost;
        Level.i.onGameWon += OnGameWon;
        Level.i.onStateChange += OnStateChange;
        GunController.i.onAmmunitionChange += OnAmmunitionChange;

        ResetUI();
    }

    private void ResetUI()
    {
        // Set values from player settings
        OnAmmunitionChange();
        OnPlayerHealthChange();
        OnStateChange();
        OnScoreChange();

        gameOverPanel.SetActive(false);
        gameWonPanel.SetActive(false);
    }

    private void SetTextfieldText(Text element, string content)
    {
        element.text = content;
    }

    public void LoadMenu()
    {
        GameController.CurrentGameController.loadMenu();
    }

    public void LoadNextLevel()
    {
        GameController.CurrentGameController.loadNextLevel();
        ResetUI();
    }

    public void RetryLevel()
    {
        GameController.CurrentGameController.retryLevel();
        ResetUI();
    }

    #region Event Handling
    private void OnGameWon()
    {
        gameWonPanel.SetActive(true);
    }

    private void OnGameLost()
    {
        gameOverPanel.SetActive(true);
    }

    private void OnAmmunitionChange()
    {
        SetTextfieldText(playerAmmoText, "Ammo: " + _playerSettings.playerAmmunition + "/" + _playerSettings.playerMaxAmmunition);
    }

    private void OnPlayerHealthChange()
    {
        SetTextfieldText(playerHealthText, "Life: " + Mathf.Clamp(_playerSettings.playerHealth, 0.0f, _playerSettings.playerMaxHealth) + " HP");
    }

    private void OnScoreChange()
    {
        SetTextfieldText(playerScoreText, "Score: " + _playerSettings.score);
    }

    private void OnStateChange()
    {
        string text = "Active Feature: ";
        switch(_gameSettings.state)
        {
            case GameController.State.BLUR:
                text += "Blured Textures";
                break;
            case GameController.State.PARTICLES:
                text += "Particles";
                break;
            case GameController.State.VISION:
                text += "Special Vision";
                break;
            default:
                text += "No Feature";
                break;
        }

        SetTextfieldText(activeFeatureText, text);
    }

    private void OnDisable()
    {
        Level.i.onPlayerHealthChange -= OnPlayerHealthChange;
        Level.i.onScoreChange -= OnScoreChange;
        Level.i.onGameLost -= OnGameLost;
        Level.i.onGameWon -= OnGameWon;
        Level.i.onStateChange -= OnStateChange;
        GunController.i.onAmmunitionChange -= OnAmmunitionChange;
    }
    #endregion
}
