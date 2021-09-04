using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectController : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private bool playDestroyAnimation;
    [SerializeField]
    private bool playAnimatorAnimation;
    [SerializeField]
    private bool playHitSound;
    [SerializeField]
    private bool changeTexture;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;

    private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void handleHit()
    {
        if (isHit) return;

        Debug.Log(name + " got hit!");
        isHit = true;

        if (playDestroyAnimation)
        {
            StartCoroutine(Fade());
        }
        if (playAnimatorAnimation)
        {
            animator.SetBool("isHit", true);
        }
        if (playHitSound)
        {
            audioSource.Play();
        }
        if (changeTexture)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    IEnumerator Fade()
    {
        Color c;
        for (float ft = 1f; ft >= 0; ft -= 0.5f * Time.deltaTime)
        {
            c = spriteRenderer.color;
            c.a = ft;
            spriteRenderer.color = c;
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
