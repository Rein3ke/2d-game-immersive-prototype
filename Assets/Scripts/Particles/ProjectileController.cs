using System.Collections;
using Controller;
using UnityEngine;

namespace Particles
{
    /// <summary>
    /// A controller to configure particles, as well as move them in one direction.
    /// </summary>
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask;

        private ParticleSystem _particleSystem;
        private Rigidbody2D _rigidbody2D;
        private GameObject _parent;

        public bool CanHitDecoration { get; set; } = true;

        public bool CanHitEnemy { get; set; } = true;

        public bool CanHitInteractAbles { get; set; } = true;

        public bool CanHitForegroundCover { get; set; } = true;

        /// <summary>
        /// Tags of objects that can be hit.
        /// </summary>
        private enum HitObject
        {
            DECORATION, ENEMY, PLAYER, INTERACTABLE_OBJECT, FOREGROUND_COVER
        }

        private void Awake()
        {
            _parent = gameObject.transform.parent.gameObject;
        }
    
        private void Start()
        {
            _particleSystem = _parent.GetComponentInChildren<ParticleSystem>();
            if (_particleSystem == null) Debug.LogWarning("Particle System not found!");

            _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            _rigidbody2D.useFullKinematicContacts = true;

            Destroy(_parent, 4f);
        }

        /// <summary>
        /// Handles the behavior for different tags.
        /// </summary>
        /// <param name="collision">Collision object.</param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((1 << collision.gameObject.layer & layerMask) == 0) return;
        
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                    HandleHit(HitObject.ENEMY);
                    break;
                case "Decoration":
                    HandleHit(HitObject.DECORATION);
                    break;
                case "InteractableObject":
                    HandleHit(HitObject.INTERACTABLE_OBJECT);
                    break;
                case "Decoration_Foreground":
                    HandleHit(HitObject.FOREGROUND_COVER);
                    break;
            }
        }

        /// <summary>
        /// Handles a different behavior depending on the given tag.
        /// </summary>
        /// <param name="hitObject">The tag of the GameObject that was hit.</param>
        private void HandleHit(HitObject hitObject)
        {
            switch (hitObject)
            {
                case HitObject.ENEMY:
                    if (CanHitEnemy) _particleSystem.Stop();
                    break;
                case HitObject.DECORATION:
                    if (CanHitDecoration) _particleSystem.Stop();
                    break;
                case HitObject.INTERACTABLE_OBJECT:
                    if (CanHitInteractAbles) _particleSystem.Stop();
                    break;
                case HitObject.FOREGROUND_COVER:
                    if (CanHitForegroundCover)
                    {
                        Destroy(_parent);
                        _particleSystem.Stop();
                    }
                    break;
            }
        }

        /// <summary>
        /// Starts the MoveTo coroutine in a specific direction and speed.
        /// </summary>
        /// <param name="direction">Direction in which the particle effect should move.</param>
        /// <param name="movementSpeed">Speed value, how fast the particle effect should move (optional).</param>
        internal void MoveToPosition(Vector3 direction, float movementSpeed = 100f)
        {
            StartCoroutine(MoveTo(direction, movementSpeed));
        }

        /// <summary>
        /// Coroutine: 
        /// </summary>
        /// <param name="direction">Direction in which the particle effect should move.</param>
        /// <param name="movementSpeed">Speed value, how fast the particle effect should move.</param>
        /// <returns>Nothing</returns>
        private IEnumerator MoveTo(Vector3 direction, float movementSpeed)
        {
            while(Level.I.IsGameRunning)
            {
                _parent.transform.position += direction * movementSpeed * Time.deltaTime;
                yield return null;
            }
            yield return null;
        }
    }
}
