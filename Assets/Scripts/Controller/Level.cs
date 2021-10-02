using System;
using System.Collections;
using Assets;
using Enemy;
using Player;
using ScriptableObjects;
using UnityEngine;
using UserInterface;
using Random = UnityEngine.Random;

namespace Controller
{
    /// <summary>
    /// Controller to create and control a level.
    /// </summary>
    public class Level : MonoBehaviour
    {
        private const string SpawnEnemyTag = "Spawn_Enemy";
        private const string SpawnInteractableObjectTag = "InteractableObject_Position";

        /// <summary>
        /// Own instance.
        /// </summary>
        public static Level I { get; private set; }

        /// <summary>
        /// Frequently called: Very important property that ensures that processes are interrupted should it be set to false.
        /// </summary>
        public bool IsGameRunning { get; private set; }

        private GameSettings _gameSettings;
        private PlayerSettings _playerSettings;
        private MaterialChangeController _materialChangeController;

        private GameObject _activeLevelPrefab;
        private Coroutine _currentSpawnEnemiesRoutine;
        private Coroutine _currentSpawnInteractablesRoutine;

        private void Awake()
        {
            if (I != null) Debug.LogError("Too many level controllers!");
            else I = this;

            _materialChangeController = GetComponent<MaterialChangeController>();
            if (_materialChangeController == null) Debug.LogError("Error: No MaterialChangeController found!");
        }

        /// <summary>
        /// Places the level prefab in the scene and sets the correct settings for player and game depending on the game state.
        /// </summary>
        /// <param name="levelPrefab">Level Prefab that will be instantiate next.</param>
        /// <param name="state">Current Game State</param>
        public void BuildLevel(GameObject levelPrefab, GameController.State state)
        {
            ResetLevel();

            _activeLevelPrefab = Instantiate(levelPrefab);

            PlayerUIController playerUIController = FindObjectOfType<PlayerUIController>();
            if (FindObjectOfType<PlayerUIController>() == null)
            {
                playerUIController = Instantiate(GameAssets.I.playerUIPrefab).GetComponentInChildren<PlayerUIController>();
            }

            switch (state)
            {
                case GameController.State.DEFAULT:
                    _playerSettings = GameAssets.I.playerSettings_default;
                    _gameSettings = GameAssets.I.gameSettings_default;
                    InstantiateTutorial(true, true);
                    break;
                case GameController.State.BLUR:
                    _playerSettings = GameAssets.I.playerSettings_default;
                    _gameSettings = GameAssets.I.gameSettings_default;
                    break;
                case GameController.State.DOODLE:
                    _playerSettings = GameAssets.I.playerSettings_default;
                    _gameSettings = GameAssets.I.gameSettings_default;
                    break;
                case GameController.State.VISION:
                    _playerSettings = GameAssets.I.playerSettings_default;
                    _gameSettings = GameAssets.I.gameSettings_default;
                    InstantiateTutorial(true, false);
                    break;
                default:
                    Debug.LogError("No feature set!");
                    break;
            }

            playerUIController.PlayerSettings = _playerSettings;
            playerUIController.GameSettings = _gameSettings;

            GunController.I.PlayerSettings = _playerSettings;
            GameController.Instance.InputController.PlayerSettings = _playerSettings;
            _materialChangeController.GameSettings = _gameSettings;

            _gameSettings.state = state;
            StateChange();
        }

        /// <summary>
        /// Loads the TutorialUI from the resources and instantiates it as a GameObject.
        /// </summary>
        /// <param name="showRightMouseButton">Should the Right Mouse Button hint be displayed?</param>
        /// <param name="showSpacebar">Should the Spacebar hint be displayed?</param>
        private void InstantiateTutorial(bool showRightMouseButton, bool showSpacebar)
        {
            var tutorialUI = Instantiate(Resources.Load("TutorialUICanvas") as GameObject);

            var tutorialUIController = tutorialUI.GetComponent<TutorialUIController>();
            if (tutorialUIController == null) return;

            tutorialUIController.PlayerSettings = _playerSettings;
            if (showRightMouseButton) tutorialUIController.ShowRightMouseButton = true;
            if (showSpacebar) tutorialUIController.ShowSpacebar = true;
        }

        /// <summary>
        /// Stops all active coroutines. Deletes current Level Prefab.
        /// Destroys all Enemies and spawnable Interactable Objects. Resets Enemy counter.
        /// </summary>
        private void ResetLevel()
        {
            if (_currentSpawnEnemiesRoutine != null)
            {
                StopCoroutine(_currentSpawnEnemiesRoutine);
                _currentSpawnEnemiesRoutine = null;
            }

            if (_currentSpawnInteractablesRoutine != null)
            {
                StopCoroutine(_currentSpawnInteractablesRoutine);
                _currentSpawnInteractablesRoutine = null;
            }

            // Delete current level
            if (_activeLevelPrefab != null) Destroy(_activeLevelPrefab);

            // Remove all active enemies
            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            foreach (EnemyController enemy in enemies)
            {
                Destroy(enemy.gameObject);
            }

            // Remove all active interactive objects
            InteractableObjectController[] interactables = FindObjectsOfType<InteractableObjectController>();
            foreach (InteractableObjectController interactable in interactables)
            {
                Destroy(interactable.gameObject);
            }

            // Reset Enemy Counter (used for Enemy Spawning)
            EnemyController.Count = 0;

            // Delete Tutorial UI GameObject & Reset Counters
            var tutorialUIGameObject = FindObjectOfType<TutorialUIController>()?.gameObject;
            if (tutorialUIGameObject != null) Destroy(tutorialUIGameObject);
            if (_playerSettings != null)
            {
                _playerSettings.SpacebarPressedCount = 0;
                _playerSettings.RightClickedCount = 0;
            }
        }

        /// <summary>
        /// Sets IsGameRunning to true. Resets current Player Settings to it's default.
        /// Starts Enemy and Interactable Object Spawn behaviour.
        /// </summary>
        public void StartLevel()
        {
            IsGameRunning = true;

            // Player Settings Reset
            _playerSettings.OnEnable();

            _currentSpawnEnemiesRoutine = StartCoroutine(SpawnEnemies());
            _currentSpawnInteractablesRoutine = StartCoroutine(SpawnInteractables());
        }

        /// <summary>
        /// Coroutine: Spawns enemies while IsGameRunning is true and enemy spawn maximum isn't reached. 
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator SpawnEnemies()
        {
            while (IsGameRunning)
            {
                float waitForSeconds = Random.Range(
                    _gameSettings.enemySpawnMinimumCooldown,
                    _gameSettings.enemySpawnMaximumCooldown
                );
                yield return new WaitForSeconds(waitForSeconds);

                // Do not spawn more enemies once the maximum number of enemies is reached.
                if (EnemyController.Count < _gameSettings.maxEnemies && IsGameRunning)
                {
                    GameObject enemyPrefab = _gameSettings.enemyPrefabs[Random.Range(0, _gameSettings.enemyPrefabs.Count)];
                    GameObject enemy = Instantiate(enemyPrefab, GetRandomEnemySpawnPosition(), Quaternion.identity, null);
                    // Enemy Events
                    enemy.GetComponent<EnemyController>().onEnemyDeath += OnEnemyDeath;
                    enemy.GetComponent<EnemyController>().onPlayerHit += OnPlayerHit;
                }
            }

            yield return null;
        }

        /// <summary>
        /// Coroutine: Periodically spawn interactive objects while the game is running.
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator SpawnInteractables()
        {
            while (IsGameRunning)
            {
                float waitForSeconds = Random.Range(_gameSettings.objectSpawnMinimumCooldown,
                    _gameSettings.objectSpawnMaximumCooldown);
                yield return new WaitForSeconds(waitForSeconds);

                GameObject interactablePrefab =
                    _gameSettings.spawnableGameObjects[Random.Range(0, _gameSettings.spawnableGameObjects.Count)];
                GameObject interactableGameObject = Instantiate(interactablePrefab, GetRandomInteractableSpawnPosition(),
                    Quaternion.identity, null);

                Destroy(interactableGameObject, waitForSeconds);
                yield return null;
            }

            yield return null;
        }

        /// <summary>
        /// Searches the scene for all Interactable Object spawn locations and returns one at random.
        /// </summary>
        /// <returns>Position from a random Interactable Object Spawner as Vector3</returns>
        private Vector3 GetRandomInteractableSpawnPosition()
        {
            GameObject[] objectSpawns = GameObject.FindGameObjectsWithTag(SpawnInteractableObjectTag);
            if (objectSpawns.Length > 0)
            {
                return objectSpawns[Random.Range(0, objectSpawns.Length)].transform.position;
            }
            else
            {
                Debug.LogError("Error: No object spawns found!");
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Searches the scene for all enemy spawn locations and returns one at random.
        /// </summary>
        /// <returns>Position from a random Enemy Spawner as Vector3</returns>
        private Vector3 GetRandomEnemySpawnPosition()
        {
            GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag(SpawnEnemyTag);
            if (enemySpawns.Length > 0)
            {
                return enemySpawns[Random.Range(0, enemySpawns.Length)].transform.position;
            }
            else
            {
                Debug.LogError("Error: No enemy spawns found!");
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Adds a value to the sum score. Calls the ScoreChange event.
        /// </summary>
        /// <param name="value">Value that should be added to the score.</param>
        public void AddToScore(float value)
        {
            _playerSettings.Score += value;
            ScoreChange();

            if (CheckScoreWinCondition())
            {
                IsGameRunning = false;
                GameWon();
            }
        }

        private bool CheckIsPlayerAlive()
        {
            return !(_playerSettings.PlayerHealth <= 0.0f);
        }

        private bool CheckScoreWinCondition()
        {
#if UNITY_EDITOR
            // If in Unity Editor, set the needed score to win to 200.
            // Otherwise it would take too much time to test each level.
            if (_playerSettings.Score >= 200f)
            {
                return true;
            }
#endif
            return _playerSettings.Score >= _gameSettings.ScoreToBeAchieved;
        }

        /// <summary>
        /// Deals damage to the player. Triggers the PlayerHealthChange event. Checks if the player is still alive.
        /// </summary>
        /// <param name="damage">Value that describes how much damage the player should receive.</param>
        internal void TakeDamage(float damage)
        {
            _playerSettings.PlayerHealth -= damage;
            _playerSettings.PlayerHealth = Mathf.Clamp(_playerSettings.PlayerHealth, 0.0f, _playerSettings.playerMaxHealth);
            PlayerHealthChange();

            if (!CheckIsPlayerAlive())
            {
                IsGameRunning = false;
                GameLost();
            }
        }

        #region Event Handling

        private void OnPlayerHit(float damage)
        {
            TakeDamage(damage);
        }

        private void OnEnemyDeath(GameObject enemy, float score)
        {
            AddToScore(score);

            enemy.GetComponent<EnemyController>().onEnemyDeath -= OnEnemyDeath;
            enemy.GetComponent<EnemyController>().onPlayerHit -= OnPlayerHit;
        }

        #endregion

        #region Events

        public event Action onPlayerHealthChange;

        private void PlayerHealthChange()
        {
            onPlayerHealthChange?.Invoke();
        }

        public event Action onScoreChange;

        private void ScoreChange()
        {
            onScoreChange?.Invoke();
        }

        public event Action onGameWon;

        private void GameWon()
        {
            onGameWon?.Invoke();
        }

        public event Action onGameLost;

        private void GameLost()
        {
            onGameLost?.Invoke();
        }

        public event Action onStateChange;

        private void StateChange()
        {
            onStateChange?.Invoke();
        }

        #endregion
    }
}