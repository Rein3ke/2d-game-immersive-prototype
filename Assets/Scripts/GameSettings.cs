using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Setting")]
public class GameSettings : ScriptableObject, ISerializationCallbackReceiver
{
    [Header("Win Conditions")]
    public int EnemiesToKill;
    public float ScoreToBeAchieved;

    [Header("Enemy Behaviour")]
    public int maxEnemies;
    [Space(8)]
    [Tooltip("Maximum value must be larger than the minimum.")]
    public float enemySpawnMinimumCooldown;
    [Tooltip("Maximum value must be larger than the minimum.")]
    public float enemySpawnMaximumCooldown;
    [Space(8)]
    public List<GameObject> enemyPrefabs;

    public void OnAfterDeserialize()
    {
        if (enemySpawnMinimumCooldown > enemySpawnMaximumCooldown) enemySpawnMinimumCooldown = enemySpawnMaximumCooldown;
    }

    public void OnBeforeSerialize()
    {
        
    }
}
