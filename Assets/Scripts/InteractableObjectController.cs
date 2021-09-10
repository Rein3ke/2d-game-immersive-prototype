using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectController : MonoBehaviour
{
    [SerializeField]
    InteractableObjectSettings interactableObjectSettings;
    [SerializeField]
    private bool _playDestroyAnimation = false;

    private bool _playAnimatorAnimation = false;
    private bool _playHitSound = false;
    private bool _changeTexture = false;
    private bool _isHit = false;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private SoundController _soundController;
    private GameController _gameController;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameController.CurrentGameController;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (interactableObjectSettings.animationController != null)
        {
            _animator = gameObject.AddComponent<Animator>();
            _animator.runtimeAnimatorController = interactableObjectSettings.animationController;
            _playAnimatorAnimation = true;
        }
        if (interactableObjectSettings.hitSoundClip != null)
        {
            _soundController = GameController.CurrentGameController.SoundController;
            _playHitSound = true;
        }
        if (interactableObjectSettings.sprite != null)
        {
            _changeTexture = true;
        }
    }

    public void handleHit()
    {
        if (_isHit) return;

        _isHit = true;

        if (_playDestroyAnimation)
        {
            StartCoroutine(Fade());
        }
        if (_playAnimatorAnimation)
        {
            _animator.SetBool("isHit", true);
        }
        if (_playHitSound)
        {
            _soundController.playAudio(interactableObjectSettings.hitSoundClip, true);
        }
        if (_changeTexture)
        {
            _spriteRenderer.sprite = interactableObjectSettings.sprite;
        }
    }

    IEnumerator Fade()
    {
        Color color;
        for (float alphaValue = 1f; alphaValue >= 0; alphaValue -= 0.5f * Time.deltaTime)
        {
            color = _spriteRenderer.color;
            color.a = alphaValue;
            _spriteRenderer.color = color;
            yield return null;
        }

        GameObject parent = gameObject.transform.parent.gameObject;

        if (parent.CompareTag("InteractableObject")) Destroy(parent.gameObject);
        else Destroy(gameObject);

        yield return null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
