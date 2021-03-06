using System;
using System.Collections;
using Controller;
using Player;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    /// <summary>
    /// A controller to adjust values in the Player UI at runtime.
    /// </summary>
    public class PlayerUIController : MonoBehaviour
    {
        [SerializeField] private Text playerHealthText;
        [SerializeField] private Text playerAmmoText;
        [SerializeField] private Text playerScoreText;
        [SerializeField] private Text activeFeatureText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject gameWonPanel;
        [SerializeField] private Image image;
        [SerializeField] private Color damageColor;
        [SerializeField] private Color healthColor;

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

        float _localHealthReference;

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
            Level.I.onPlayerHealthChange += OnPlayerHealthChange;
            Level.I.onScoreChange += OnScoreChange;
            Level.I.onGameLost += OnGameLost;
            Level.I.onGameWon += OnGameWon;
            Level.I.onStateChange += OnStateChange;
            GunController.I.onAmmunitionChange += OnAmmunitionChange;

            ResetUI();
        }

        private void ResetUI()
        {
            _localHealthReference = _playerSettings.PlayerHealth;

            // Set values from player settings
            OnAmmunitionChange();
            OnPlayerHealthChange();
            OnStateChange();
            OnScoreChange();

            gameOverPanel.SetActive(false);
            gameWonPanel.SetActive(false);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

            // Level Blend In Effect
            StartCoroutine(ShowColorOverlay(Color.black, 0.2f));
        }

        private void SetTextfieldText(Text element, string content)
        {
            element.text = content;
        }

        #region Button Handler Methods
        public void LoadMenu()
        {
            GameController.Instance.LoadMenu();
        }

        public void LoadNextLevel()
        {
            GameController.Instance.LoadNextLevel();
            ResetUI();
        }

        public void RetryLevel()
        {
            GameController.Instance.RetryLevel();
            ResetUI();
        }
        #endregion

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
            SetTextfieldText(playerAmmoText, "Ammo: " + _playerSettings.PlayerAmmunition + "/" + _playerSettings.playerMaxAmmunition);
        }

        /// <summary>
        /// Called when the Health value changes. Displays an overlay when the value changes.
        /// </summary>
        private void OnPlayerHealthChange()
        {
            SetTextfieldText(playerHealthText, "Life: " + Mathf.Clamp(_playerSettings.PlayerHealth, 0.0f, _playerSettings.playerMaxHealth) + " HP");
            if (_playerSettings.PlayerHealth < _localHealthReference)
            {
                StartCoroutine(ShowColorOverlay(damageColor, 0.5f));
            } else if (_playerSettings.PlayerHealth > _localHealthReference)
            {
                StartCoroutine(ShowColorOverlay(healthColor, 0.5f));
            }
            _localHealthReference = _playerSettings.PlayerHealth;
        }

        /// <summary>
        /// Coroutine: Animates the color of the overlay from 0.5f to 0.0f in a given time.
        /// </summary>
        /// <param name="color">Color to be used.</param>
        /// <param name="speed">Animation speed.</param>
        /// <returns></returns>
        private IEnumerator ShowColorOverlay(Color color, float speed)
        {
            image.color = color;
            Color c;
            for (float alpha = .5f; alpha >= 0f; alpha -= speed * Time.deltaTime)
            {
                c = image.color;
                c.a = alpha;
                image.color = c;
                yield return null;
            }
            yield return null;
        }

        private void OnScoreChange()
        {
            SetTextfieldText(playerScoreText, "Score: " + _playerSettings.Score);
        }

        private void OnStateChange()
        {
            string text = "Active Feature: " + Enum.GetName(typeof (GameController.State), _gameSettings.state);

            SetTextfieldText(activeFeatureText, text);
        }

        private void OnDisable()
        {
            Level.I.onPlayerHealthChange -= OnPlayerHealthChange;
            Level.I.onScoreChange -= OnScoreChange;
            Level.I.onGameLost -= OnGameLost;
            Level.I.onGameWon -= OnGameWon;
            Level.I.onStateChange -= OnStateChange;
            GunController.I.onAmmunitionChange -= OnAmmunitionChange;
        }
        #endregion
    }
}
