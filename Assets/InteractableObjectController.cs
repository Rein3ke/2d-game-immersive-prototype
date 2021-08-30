using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectController : MonoBehaviour
{
    [SerializeField]
    private bool playDissolveAnimation;

    private SpriteRenderer spriteRenderer;

    private bool isHit;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

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
