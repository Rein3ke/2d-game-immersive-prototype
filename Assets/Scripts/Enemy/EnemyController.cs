using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// A controller to control the enemy behavior. A controller to control the enemy behavior. In addition, animation and sound are controlled here, which were previously stored in the enemy settings.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyController : MonoBehaviour, IHitAble
    {
        /// <summary>
        /// A property to monitor the current number of enemies.
        /// </summary>
        public static int Count { get; set; }

        public EnemySettings EnemySettings => _enemySettings;

        [SerializeField]
        private EnemySettings _enemySettings;
        [SerializeField]
        private LayerMask _layerMask;

        private GameController _gameController;

        private bool _isHit;
        private SpriteRenderer SpriteRenderer { get; set; }

        private SoundController _soundController;
        private Animator _animator;
        private BoxCollider2D _boxCollider2D;
        private Coroutine _currentShootRoutine;
        private Camera _mainCamera;
    
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");

        /// <summary>
        /// Standard unity method. Set all the necessary components.
        /// </summary>
        private void Start()
        {
            _mainCamera = Camera.main;
            _gameController = GameController.Instance;

            // Set sprite renderer
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (SpriteRenderer == null) Debug.LogError("Error: SpriteRenderer not found!");

            SpriteRenderer.sprite = _enemySettings.sprite;
            SpriteRenderer.color = _enemySettings.color;

            // Set box collider 2d
            _boxCollider2D = GetComponentInChildren<BoxCollider2D>();
            if (_boxCollider2D == null) Debug.LogError("Error: BoxCollider2D not found!");

            // Set animator
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null) Debug.LogError("Error: Animator not found!");

            // Set sound controller
            _soundController = _gameController.SoundController;
            if (_soundController == null) Debug.LogError("Error: SoundController not found!");

            EnemySettingsErrorCheck();

            // Increase global enemy counter
            Count++;

            StartCoroutine(EnemyBehaviour());
        }

        /// <summary>
        /// Used to check the set values in the EnemySettings.
        /// </summary>
        private void EnemySettingsErrorCheck()
        {
            if (_enemySettings.switchPositionTime <= 0) Debug.LogWarning("Warning: SwitchPositionTime not set!");
        }

        /// <summary>
        /// Coroutine: Controls the general behaviour in intervals.
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator EnemyBehaviour()
        {
            // Play random spawn sound
            PlayRandomAudioFromList(_enemySettings.spawnSounds, .3f, true);

            while (Level.I.IsGameRunning && !_isHit)
            {
                GameObject[] covers = GameObject.FindGameObjectsWithTag("Position_Cover");
            
                // Find cover and start coroutine
                if (covers.Length > 0)
                {
                    var randomCoverIndex = Random.Range(0, covers.Length);
                    var coverPosition = covers[randomCoverIndex].transform.position;

                    StartCoroutine(MoveToPosition(coverPosition));
                }
                else
                {
                    Debug.LogError("Error: No cover found!");
                }
            
                yield return new WaitForSeconds(20f);
            }
            yield return null;
        }

        /// <summary>
        /// Coroutine: A method with logic that stops current behavior, lets the GameObject move to a position, and then resumes the stopped behavior.
        /// </summary>
        /// <param name="position">Target position</param>
        /// <returns>Nothing</returns>
        private IEnumerator MoveToPosition(Vector3 position)
        {
            // Get current position
            var currentPosition = transform.position;

            // Set different x position at target
            var targetPosition = new Vector3(
                position.x + Random.Range(-1, 1),
                position.y,
                position.z
            );

            // Flip sprite if needed
            FlipSpriteBasedOnDirection(currentPosition, targetPosition);

            // Stop active shoot coroutine
            if (_currentShootRoutine != null)
            {
                StopCoroutine(_currentShootRoutine);
            }

            // Set animator variable IsRunning to true
            _animator.SetBool(IsRunning, true);
        
            // Set local time variables
            var elapsedTime = 0.0f;
            var switchPositionTime = _enemySettings.switchPositionTime;
        
            // True as long as the required time is not reached.
            while ((elapsedTime < switchPositionTime) && !_isHit)
            {
                transform.position = Vector3.Lerp(currentPosition, targetPosition, (elapsedTime / switchPositionTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Set animator variable IsRunning to false
            _animator.SetBool(IsRunning, false);

            // Start shoot coroutine
            _currentShootRoutine = StartCoroutine(Shoot());
        
            yield return null;
        }

        /// <summary>
        /// Flips the sprite based on the x value.
        /// </summary>
        /// <param name="originPosition">Origin position (current position)</param>
        /// <param name="targetPosition">Target position</param>
        private void FlipSpriteBasedOnDirection(Vector3 originPosition, Vector3 targetPosition)
        {
            var localDirection = transform.InverseTransformDirection(originPosition - targetPosition);
            var transformLocal = transform.localScale;
            var localScale = localDirection.x < 0 ? new Vector3(-1, transformLocal.y) : new Vector3(1, transformLocal.y);
            transform.localScale = localScale;
        }

        /// <summary>
        /// Coroutine: Stops current behavior and changes transparency of color in SpriteRenderer.
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator FadeOut()
        {
            if (_currentShootRoutine != null)
            {
                StopCoroutine(_currentShootRoutine);
            }

            for (var ft = 1f; ft >= 0; ft -= 0.5f * Time.deltaTime)
            {
                var c = SpriteRenderer.color;
                c.a = ft;
                SpriteRenderer.color = c;
                yield return null;
            }

            Destroy(gameObject);
            yield return null;
        }

        /// <summary>
        /// Coroutine: Controls the shooting behavior. It runs until it is interrupted.
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator Shoot()
        {
            while(!_isHit && Level.I.IsGameRunning)
            {
                PlayRandomAudioFromList(_enemySettings.shootingSounds, .1f, true);

                var playerGameObject = GameObject.FindGameObjectWithTag("Player");
                var hitObject = PerformRayCast(playerGameObject.transform.position, true);

                // Instantiate shot particle
                var transformPosition = transform.position;
                Instantiate(Resources.Load("ShootingParticle") as GameObject, new Vector3(transformPosition.x, transformPosition.y + .75f, transformPosition.z), Quaternion.identity);

                if (hitObject == null)
                {
                    PlayRandomAudioFromList(_enemySettings.hitNothingSounds, .3f, true);
                }
                else
                {
                    // Continue based on hit object tag
                    switch (hitObject.tag)
                    {
                        case "Decoration_Foreground":
                            PlayRandomAudioFromList(_enemySettings.hitObjectSounds, 1.0f, true);
                            break;
                        case "Player":
                            PlayRandomAudioFromList(_enemySettings.hitPlayerSounds, 1.0f, true);
                            PlayerHit(_enemySettings.damage);
                            break;
                    }
                }

                yield return new WaitForSeconds(_enemySettings.shootingInterval);
            }
            yield return null;
        }

        /// <summary>
        /// Performs a raycast in the direction of the target position and returns the hit object. Plays a shot particle.
        /// </summary>
        /// <param name="targetPosition">Target position (player)</param>
        /// <param name="enableAccuracy">If true, the shot is inaccurate based on the spread factor</param>
        /// <returns></returns>
        private GameObject PerformRayCast(Vector3 targetPosition, bool enableAccuracy)
        {
            var transformPosition = transform.position;
            var origin = new Vector3(transformPosition.x, transformPosition.y + 1.25f, transformPosition.z);
        
            if (enableAccuracy)
            {
                targetPosition = new Vector3(
                    targetPosition.x + Random.Range(-_enemySettings.spreadFactor, _enemySettings.spreadFactor),
                    targetPosition.y + Random.Range(-_enemySettings.spreadFactor, _enemySettings.spreadFactor),
                    targetPosition.z + Random.Range(-_enemySettings.spreadFactor, _enemySettings.spreadFactor)
                );
            }
        
            var direction = targetPosition - origin;
            var ray = new Ray(origin, direction);

            // Projectile Effect
            var projectile = Instantiate(Resources.Load("Projectile") as GameObject, origin, Quaternion.identity);
            if (projectile != null)
            {
                var projectileController = projectile.GetComponentInChildren<ProjectileController>();
                projectileController.CanHitDecoration = false;
                projectileController.CanHitEnemy = false;
                var projectileDirection = _mainCamera.transform.position - origin;
                projectileController.MoveToPosition(projectileDirection, 5f);
            }

            Debug.DrawRay(ray.origin, ray.direction, Color.red, 2f);
        
            // Get ray intersection
            var hit2D = Physics2D.GetRayIntersection(ray, 50f, _layerMask);

            // If hit, return game object
            return hit2D.collider != null ? hit2D.collider.gameObject : null;
        }

        /// <summary>
        /// Plays a random audio clip from the passed list via the SoundController.
        /// </summary>
        /// <param name="audioClipList">List of audio files that should be played</param>
        /// <param name="volume">Volume between 0.0f and 1.0f</param>
        /// <param name="pitch">Should the audio file played with a pitch</param>
        private void PlayRandomAudioFromList(IReadOnlyList<AudioClip> audioClipList, float volume, bool pitch)
        {
            if (audioClipList.Count == 0)
            {
                Debug.LogError("Error: AudioClip list is empty!");
                return;
            }

            var randomAudioClip = audioClipList[Random.Range(0, audioClipList.Count)];
            _soundController.PlayAudio(randomAudioClip, volume, pitch);
        }

        /// <summary>
        /// Called after the GameObject has been hit by a raycast. Stops all running coroutines and plays some effects. Calls EnemyDeath() at the end.
        /// </summary>
        public void HandleHit()
        {
            if (_isHit) return;
            // Set isHit to prevent getting hit again
            _isHit = true;
            // Stop all behaviours like shooting or walking
            StopAllCoroutines();
            // Instantiate a particle effect after hit
            Instantiate(Resources.Load("HitParticle") as GameObject, transform.position, Quaternion.identity);
            // Play fade out animation
            StartCoroutine(FadeOut());
            // Set animator variable "IsDead" to change to death animation
            _animator.SetBool(IsDead, true);
            // Play a random death audio clip
            PlayRandomAudioFromList(_enemySettings.deathSounds, 1f, true);
            // Run destroy method
            EnemyDeath();
        }

        #region Events
    
        public delegate void EnemyDeathCallback(GameObject gameObject, float score);
        public event EnemyDeathCallback onEnemyDeath;
        /// <summary>
        /// Calls the onEnemyDeath if enemy was hit.
        /// </summary>
        private void EnemyDeath()
        {
            onEnemyDeath?.Invoke(gameObject, _enemySettings.scoreForGettingKilled);
        }

        public delegate void PlayerHitCallback(float damagePoints);
        public event PlayerHitCallback onPlayerHit;
        /// <summary>
        /// Calls the onPlayerHit event with the Damage value set.
        /// </summary>
        /// <param name="damagePoints">Damage dealt</param>
        private void PlayerHit(float damagePoints)
        {
            onPlayerHit?.Invoke(damagePoints);
        }
    
        #endregion

        /// <summary>
        /// Decrease global enemy counter if destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Count--;
        }
    }
}
