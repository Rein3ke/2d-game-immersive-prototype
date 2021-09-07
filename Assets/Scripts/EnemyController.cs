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
    private bool playHitSound;
    [SerializeField]
    private float walkSpeed = 3.0f;
    [SerializeField]
    private float spreadFactor = 4f;
    [SerializeField]
    private float causedDamage = 5f;
    [SerializeField]
    private LayerMask layerMask;
    // Audio
    [SerializeField]
    private AudioClip shootingAudioClip;
    [SerializeField]
    private AudioClip spawnAudioClip;
    [SerializeField]
    private AudioClip deathAudioClip;
    [SerializeField]
    private AudioClip missedBulletAudioClip;
    [SerializeField]
    private AudioClip bulletImpactCoverAudioClip;
    [SerializeField]
    private AudioClip bulletImpactPlayerAudioClip;

    private bool isHit = false;
    private bool isActive = true;
    private SpriteRenderer spriteRenderer;
    private SoundController soundController;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        soundController = GameController.CurrentGameController.SoundController;

        _count++;

        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        if (covers.Length > 0)
        {
            int randomCoverIndex = Random.Range(0, covers.Length);
            Vector3 position = covers[randomCoverIndex].transform.position;
            StartCoroutine(GoToPosition(position));
            soundController.playAudio(spawnAudioClip, .3f, true);
        } else
        {
            Debug.LogError("No cover found!");
        }

        GameController.CurrentGameController.onPlayerDeath += OnPlayerDeath;
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

        animator.SetBool("isRunning", true);

        while (elapsedTime < waitTime && !isHit)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("isRunning", false);

        StartCoroutine(Shoot());
        yield return null;
    }

    private IEnumerator FadeOut()
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
        while(!isHit && isActive)
        {
            soundController.playAudio(shootingAudioClip, .1f, true);

            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            GameObject hitObject = PerformRayCast(playerGameObject.transform.position, true);

            if (hitObject == null)
            {
                for (int i = 0; i < 3; i++)
                {
                    soundController.playAudio(missedBulletAudioClip, true);
                }
            }
            else
            {
                switch (hitObject.tag)
                {
                    case "Decoration":
                        soundController.playAudio(bulletImpactCoverAudioClip, true);
                        break;
                    case "Player":
                        soundController.playAudio(bulletImpactPlayerAudioClip, true);
                        PlayerHit(causedDamage);
                        break;
                }
            }

            yield return new WaitForSeconds(3f);
        }
        yield return null;
    }

    private GameObject PerformRayCast(Vector3 targetPosition, bool enableAccuracy)
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z);
        
        if (enableAccuracy)
        {
            targetPosition = new Vector3(
                targetPosition.x + Random.Range(-spreadFactor, spreadFactor),
                targetPosition.y + Random.Range(-spreadFactor, spreadFactor),
                targetPosition.z + Random.Range(-spreadFactor, spreadFactor)
                );
        }
        
        Vector3 direction = targetPosition - origin;
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 2f);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 50f, layerMask);

        if (hit2D.collider != null)
        {
            Debug.Log(hit2D.collider.name);
            return hit2D.collider.gameObject;
        }
        else return null;
    }

    internal void handleHit()
    {
        if (isHit) return;

        isHit = true;

        StopAllCoroutines();

        if (playDestroyAnimation)
        {
            StartCoroutine(FadeOut());
            animator.SetBool("isDead", true);
        }
        if (playHitSound)
        {
            soundController.playAudio(deathAudioClip, .3f, false);
        }

        EnemyDeath();
    }

    private void OnPlayerDeath()
    {
        isActive = false;
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

    public delegate void PlayerHitCallback(float damagePoints);
    public event PlayerHitCallback onPlayerHit;
    public void PlayerHit(float damagePoints)
    {
        if (onPlayerHit != null)
        {
            onPlayerHit(damagePoints);
        }
    }

    private void OnDestroy()
    {
        _count--;
        GameController.CurrentGameController.onPlayerDeath -= OnPlayerDeath;
    }
}
