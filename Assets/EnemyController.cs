using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static int Count
    {
        get => _count;
    }
    private static int _count = 0;

    [SerializeField]
    private bool playDestroyAnimation;
    [SerializeField]
    private float walkSpeed = 3.0f;
    [SerializeField]
    private AudioClip shootingAudioClip;
    [SerializeField]
    private float spreadFactor = 40f;
    [SerializeField]
    private LayerMask layerMask;
    private bool isHit;
    private SpriteRenderer spriteRenderer;
    private SoundController soundController;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        soundController = GameController.CurrentGameController.SoundController;

        _count++;

        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        if (covers.Length > 0)
        {
            int randomCoverIndex = Random.Range(0, covers.Length);
            Debug.Log("Cover: " + randomCoverIndex + " : " + covers.Length);
            Vector3 position = covers[randomCoverIndex].transform.position;
            StartCoroutine(GoToPosition(position));
        } else
        {
            Debug.LogError("No cover found!");
        }
    }

    private IEnumerator GoToPosition(Vector3 position)
    {
        Vector3 currentPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            position.x + Random.Range(-1, 1),
            position.y,
            position.z
            );

        // Flip sprite based on the x value
        Vector3 localDirection = transform.InverseTransformDirection(currentPosition - targetPosition);
        if (localDirection.x < 0) spriteRenderer.flipX = true;


        float elapsedTime = 0;
        float waitTime = walkSpeed;

        StopCoroutine(Shoot());

        while (elapsedTime < waitTime && !isHit)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Shoot());
        yield return null;
    }

    private IEnumerator Fade()
    {
        StopCoroutine(Shoot());

        Color c;
        for (float ft = 1f; ft >= 0; ft -= 0.5f * Time.deltaTime)
        {
            c = spriteRenderer.color;
            c.a = ft;
            spriteRenderer.color = c;
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator Shoot()
    {
        while(!isHit)
        {
            shootSalve();
            yield return new WaitForSeconds(3f);
        }
        yield return null;
    }

    private void shootSalve()
    {
        Vector3 playerPosition = Camera.main.transform.position;

        playerPosition.x += Random.Range(-spreadFactor, spreadFactor);
        playerPosition.y += Random.Range(-spreadFactor, spreadFactor);
        playerPosition.z += Random.Range(-spreadFactor, spreadFactor);

        Ray ray = Camera.main.ScreenPointToRay(playerPosition);
        Debug.DrawLine(ray.origin, ray.direction, Color.green, .5f, false);

        soundController.playAudio(shootingAudioClip, true);

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 20.0f, layerMask);

        if (hit2D.collider != null)
        {
            Debug.Log(hit2D.collider.name);
        }
    }

    internal void handleHit()
    {
        if (isHit) return;

        Debug.Log(name + " got hit!");
        isHit = true;

        StopAllCoroutines();

        if (playDestroyAnimation)
        {
            StartCoroutine(Fade());
        }
        /*if (playAnimatorAnimation)
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
        }*/
        EnemyDeath();
    }

    public delegate void EnemyDeathCallback(GameObject gameObject);
    public event EnemyDeathCallback onEnemyDeath;
    public void EnemyDeath()
    {
        if (onEnemyDeath != null)
        {
            onEnemyDeath(gameObject);
        }
    }

    private void OnDestroy()
    {
        _count--;
    }
}
