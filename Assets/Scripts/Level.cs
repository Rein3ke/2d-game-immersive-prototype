using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    public static Level i
    {
        get => _i;
    }
    private static Level _i;

    private GameSettings _gameSettings;
    private PlayerSettings _playerSettings;
    private GameController _gameController;

    private bool _isGameRunning;

    private void Awake()
    {
        if (_i != null) Debug.LogError("Too many level controllers!");
        else _i = this;
    }

    private void Start()
    {
        _gameController = GameController.CurrentGameController;
        _gameController.onGameEnd += OnGameEnd;
        _gameController.onGameWon += OnGameEnd;
    }

    public void BuildLevel(GameObject levelPrefab, GameController.Phase phase)
    {
        Instantiate(levelPrefab);
        Instantiate(GameAssets.i.playerUIPrefab, new Vector3(0f, 0f, -20f), Quaternion.identity);

        switch (phase)
        {
            case GameController.Phase.DEFAULT:
                _playerSettings = GameAssets.i.playerSettings_default;
                _gameSettings = GameAssets.i.gameSettings_default;
                break;
        }
    }

    public void StartLevel()
    {
        _isGameRunning = true;

        // Player Settings Reset
        _playerSettings.OnEnable();

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (_isGameRunning)
        {
            float waitForSeconds = Random.Range(_gameSettings.enemySpawnMinimumCooldown, _gameSettings.enemySpawnMaximumCooldown);
            yield return new WaitForSeconds(waitForSeconds);

            // Do not spawn more enemies once the maximum number of enemies is reached.
            if (EnemyController.Count < _gameSettings.maxEnemies && _isGameRunning)
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

    private void OnPlayerHit(float damage)
    {
        _playerSettings.playerHealth -= damage;
        PlayerHealthChange();
        CheckIsPlayerDead();
    }

    private void CheckIsPlayerDead()
    {
        if (_playerSettings.playerHealth <= 0.0f)
        {
            OnGameEnd();
        }
    }

    private void OnEnemyDeath(GameObject enemy, float score)
    {
        _gameController.AddToScore(score);

        enemy.GetComponent<EnemyController>().onEnemyDeath -= OnEnemyDeath;
        enemy.GetComponent<EnemyController>().onPlayerHit -= OnPlayerHit;
    }

    private Vector3 GetRandomEnemySpawnPosition()
    {
        GameObject[] enemySpawns = GameObject.FindGameObjectsWithTag("Spawn_Enemy");
        if (enemySpawns.Length > 0)
        {
            return enemySpawns[Random.Range(0, enemySpawns.Length)].transform.position;
        } else
        {
            Debug.LogError("Error: No enemy spawns found!");
            return Vector3.zero;
        }
    }

    private void OnGameEnd()
    {
        _isGameRunning = false;
    }

    private void OnDisable()
    {
        _gameController.onGameEnd -= OnGameEnd;
        _gameController.onGameWon -= OnGameEnd;
    }

    public event Action onPlayerHealthChange;
    public void PlayerHealthChange()
    {
        if (onPlayerHealthChange != null)
        {
            onPlayerHealthChange();
        }
    }
}
