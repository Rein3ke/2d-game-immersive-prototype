using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectController : MonoBehaviour
{
    [SerializeField]
    private bool playDissolveAnimation;
    [SerializeField]
    private bool playAnimatorAnimation;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (animator != null) playAnimatorAnimation = true;

        CameraMovementController.Instance.onRayCastHit += handleHit;
    }

    private void handleHit(RaycastHit2D hit)
    {
        if (hit.collider.gameObject.Equals(this.gameObject) && !isHit)
        {
            Debug.Log(name + " got hit!");
            isHit = true;

            if (playDissolveAnimation)
            {
                StartCoroutine("Fade");
            }
            if (playAnimatorAnimation)
            {
                animator.SetBool("isHit", true);
            }
        }
    }

    IEnumerator Fade()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.5f * Time.deltaTime)
        {
            Color c = spriteRenderer.color;
            c.a = ft;
            spriteRenderer.color = c;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        CameraMovementController.Instance.onRayCastHit -= handleHit;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        CameraMovementController.Instance.onRayCastHit -= handleHit;
    }
}
