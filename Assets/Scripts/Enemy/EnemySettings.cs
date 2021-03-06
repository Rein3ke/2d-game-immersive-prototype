using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "Enemy Setting")]
    public class EnemySettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public float scoreForGettingKilled;
        [Header("Enemy Behaviour")]
        [Range(1f, 3f)]
        public float switchPositionTime;
        [Range(0f, 1.5f)]
        public float spreadFactor;
        [Range(5f, 15f)]
        public float damage;
        public float shootingInterval;
        [Header("Appearance")]
        public Sprite sprite;
        public Color color;
        [Space(8)]
        public List<AudioClip> spawnSounds;
        public List<AudioClip> shootingSounds;
        public List<AudioClip> deathSounds;
        public List<AudioClip> hitPlayerSounds;
        public List<AudioClip> hitObjectSounds;
        public List<AudioClip> hitNothingSounds;

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
        }
    }
}
