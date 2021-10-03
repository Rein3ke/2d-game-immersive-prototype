using System.Collections;
using ScriptableObjects;
using UnityEngine;

namespace Controller
{
    /// <summary>
    /// Controller to manage the behavior of interactive objects. Implements the IHitAble interface.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractableObjectController : MonoBehaviour, IHitAble
    {
        [SerializeField] InteractableObjectSettings _interactableObjectSettings;

        // Boolean fields
        private bool _playAnimatorAnimation;
        private bool _playHitSound;
        private bool _textureIsChangedOnHit;
        private bool _isHit;

        private Animator _animator;
        public SpriteRenderer SpriteRenderer { get; private set; }
        private SoundController _soundController;
        private Level _level;
    
        private static readonly int IsHit = Animator.StringToHash("isHit");

        /// <summary>
        /// Standard unity method. Sets the corresponding fields depending on the InteractableObjectSettings.
        /// </summary>
        private void Start()
        {
            // Get current level instance
            _level = Level.I;
        
            // Get attached sprite renderer of the game object
            SpriteRenderer = GetComponent<SpriteRenderer>();
        
            // When an animation controller is set in the InteractableObjectSettings, attach an animator to the GameObject and set the animation controller to it. 
            if (_interactableObjectSettings.animationController != null)
            {
                _animator = gameObject.AddComponent<Animator>();
                _animator.runtimeAnimatorController = _interactableObjectSettings.animationController;
                _playAnimatorAnimation = true;
            }
        
            // Set PlayHitSound to true if a corresponding clip is set in the InteractableObjectSettings.
            if (_interactableObjectSettings.hitSoundClip != null)
            {
                // Get current sound controller instance
                _soundController = GameController.Instance.SoundController;
            
                _playHitSound = true;
            }
        
            // Set _textureIsChangedOnHit to true if a sprite is set in the InteractableObjectSettings.
            if (_interactableObjectSettings.sprite != null)
            {
                _textureIsChangedOnHit = true;
            }
        
            // Perform a fade-in animation, if the gameObject is set to spawnable in the InteractableObjectSettings.
            if (_interactableObjectSettings.spawnable)
            {
                StartFadeInCoroutine();
            }
        }

        /// <summary>
        /// Executes FadeIn-Coroutine.
        /// </summary>
        private void StartFadeInCoroutine()
        {
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Controls how the interactive object should behave after a hit.
        /// </summary>
        public void HandleHit()
        {
            // If the GameObject is already hit, return
            if (_isHit) return;

            _isHit = true;

            _level.AddToScore(_interactableObjectSettings.score);

            if (_interactableObjectSettings.lifepointsGainAfterHit < 0.0f || _interactableObjectSettings.lifepointsGainAfterHit > 0.0f)
            {
                _level.TakeDamage(_interactableObjectSettings.lifepointsGainAfterHit * -1);
            }
            if (_interactableObjectSettings.playFadeOutAnimation)
            {
                StartCoroutine(FadeOutAndDestroy());
            }
            if (_playAnimatorAnimation)
            {
                _animator.SetBool(IsHit, true);
            }
            if (_playHitSound)
            {
                _soundController.PlayAudio(_interactableObjectSettings.hitSoundClip, true);
            }
            if (_textureIsChangedOnHit)
            {
                SpriteRenderer.sprite = _interactableObjectSettings.sprite;
            }
        }

        /// <summary>
        /// Coroutine: Changes the alpha value of the SpriteRender color over time to 0. When complete, the object is destroyed.
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator FadeOutAndDestroy()
        {
            Color color;
            for (float alphaValue = 1f; alphaValue >= 0; alphaValue -= 0.5f * Time.deltaTime)
            {
                color = SpriteRenderer.color;
                color.a = alphaValue;
                SpriteRenderer.color = color;
                yield return null;
            }

            Destroy(gameObject);
            yield return null;
        }

        /// <summary>
        /// Coroutine: Changes the alpha value of the SpriteRender color over time from 0 to 1.
        /// </summary>
        /// <returns>Nothing</returns>
        private IEnumerator FadeIn()
        {
            Color color;
            for (float alphaValue = 0f; alphaValue <= 1f; alphaValue += 0.5f * Time.deltaTime)
            {
                color = SpriteRenderer.color;
                color.a = alphaValue;
                SpriteRenderer.color = color;
                yield return null;
            }

            yield return null;
        }

        /// <summary>
        /// Standard unity method. Stops all coroutines and also destroys the parent Game Object.
        /// </summary>
        private void OnDestroy()
        {
            StopAllCoroutines();

            GameObject parent = gameObject.transform.parent.gameObject;
            if (parent.CompareTag("InteractableObject")) Destroy(parent.gameObject);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
