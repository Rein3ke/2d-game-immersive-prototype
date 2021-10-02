using System.Collections.Generic;
using Controller;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Game Setting")]
    public class GameSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("Game State")]
        public GameController.State state;

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

        [Header("Objects to spawn")]
        public float objectSpawnMinimumCooldown;
        public float objectSpawnMaximumCooldown;
        public float objectDecayTime;
        [Space(8)]
        public List<GameObject> spawnableGameObjects;

        public void OnAfterDeserialize()
        {
            if (enemySpawnMinimumCooldown > enemySpawnMaximumCooldown) enemySpawnMinimumCooldown = enemySpawnMaximumCooldown;
            if (objectSpawnMinimumCooldown > objectSpawnMaximumCooldown) objectSpawnMinimumCooldown = objectSpawnMaximumCooldown;
        }

        public void OnBeforeSerialize()
        {
        
        }
    }
}
