using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractableObjectController : MonoBehaviour, IHitable
{
    [SerializeField]
    InteractableObjectSettings _interactableObjectSettings;

    private bool _playAnimatorAnimation = false;
    private bool _playHitSound = false;
    private bool _changeTexture = false;
    private bool _isHit = false;

    private Animator _animator;
    public SpriteRenderer SpriteRenderer
    {
        get => _spriteRenderer;
    }
    private SpriteRenderer _spriteRenderer;
    private SoundController _soundController;
    private Level _level;

    // Start is called before the first frame update
    void Start()
    {
        _level = Level.i;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_interactableObjectSettings.animationController != null)
        {
            _animator = gameObject.AddComponent<Animator>();
            _animator.runtimeAnimatorController = _interactableObjectSettings.animationController;
            _playAnimatorAnimation = true;
        }
        if (_interactableObjectSettings.hitSoundClip != null)
        {
            _soundController = GameController.CurrentGameController.SoundController;
            _playHitSound = true;
        }
        if (_interactableObjectSettings.sprite != null)
        {
            _changeTexture = true;
        }
        if (_interactableObjectSettings.spawnable)
        {
            PrepareFadeIn();
        }
    }

    private void PrepareFadeIn()
    {
        //_spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0f);
        StartCoroutine(FadeIn());
    }

    public void handleHit()
    {
        if (_isHit) return;

        _isHit = true;

        _level.AddToScore(_interactableObjectSettings.score);

        if (_interactableObjectSettings.lifepointsGainAfterHit < 0.0f || _interactableObjectSettings.lifepointsGainAfterHit > 0.0f)
        {
            Level.i.TakeDamage(_interactableObjectSettings.lifepointsGainAfterHit * -1);
        }
        if (_interactableObjectSettings.playFadeOutAnimation)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
        if (_playAnimatorAnimation)
        {
            _animator.SetBool("isHit", true);
        }
        if (_playHitSound)
        {
            _soundController.PlayAudio(_interactableObjectSettings.hitSoundClip, true);
        }
        if (_changeTexture)
        {
            _spriteRenderer.sprite = _interactableObjectSettings.sprite;
        }
    }

    public IEnumerator FadeOutAndDestroy()
    {
        Color color;
        for (float alphaValue = 1f; alphaValue >= 0; alphaValue -= 0.5f * Time.deltaTime)
        {
            color = _spriteRenderer.color;
            color.a = alphaValue;
            _spriteRenderer.color = color;
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }

    public IEnumerator FadeIn()
    {
        Color color;
        for (float alphaValue = 0f; alphaValue <= 1f; alphaValue += 0.5f * Time.deltaTime)
        {
            color = _spriteRenderer.color;
            color.a = alphaValue;
            _spriteRenderer.color = color;
            yield return null;
        }

        yield return null;
    }

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
