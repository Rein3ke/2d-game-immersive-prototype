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
    private EnemySettings enemySettings;
    [SerializeField]
    private LayerMask layerMask;

    private bool isHit = false;
    private bool isActive = true;
    private SpriteRenderer spriteRenderer;
    private SoundController soundController;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // Set Sprite Renderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = enemySettings.sprite;
        spriteRenderer.color = enemySettings.color;

        animator = GetComponentInChildren<Animator>();
        soundController = GameController.CurrentGameController.SoundController;

        _count++;

        GameObject[] covers = GameObject.FindGameObjectsWithTag("Position_Cover");
        if (covers.Length > 0)
        {
            int randomCoverIndex = Random.Range(0, covers.Length);
            Vector3 position = covers[randomCoverIndex].transform.position;
            StartCoroutine(GoToPosition(position));
            // Play Spawn Sound (Random)
            PlayAudio(enemySettings.spawnSounds, .3f, true);
        } else
        {
            Debug.LogError("No cover found!");
        }

        GameController.CurrentGameController.onGameEnd += OnPlayerDeath;
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
        float waitTime = enemySettings.walkSpeed;

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
            PlayAudio(enemySettings.shootingSounds, .1f, true);

            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            GameObject hitObject = PerformRayCast(playerGameObject.transform.position, true);

            if (hitObject == null)
            {
                PlayAudio(enemySettings.hitNothingSounds, .3f, true);
            }
            else
            {
                switch (hitObject.tag)
                {
                    case "Cover":
                        PlayAudio(enemySettings.hitObjectSounds, 1.0f, true);
                        break;
                    case "Player":
                        PlayAudio(enemySettings.hitPlayerSounds, 1.0f, true);
                        PlayerHit(enemySettings.damage);
                        break;
                }
            }

            yield return new WaitForSeconds(enemySettings.shootingInterval);
        }
        yield return null;
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 1f)
    {
        GameObject line = new GameObject();
        line.transform.position = start;
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = .01f;
        lr.endWidth = .1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        Destroy(line, duration);
    }

    private GameObject PerformRayCast(Vector3 targetPosition, bool enableAccuracy)
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z);
        
        if (enableAccuracy)
        {
            targetPosition = new Vector3(
                targetPosition.x + Random.Range(-enemySettings.spreadFactor, enemySettings.spreadFactor),
                targetPosition.y + Random.Range(-enemySettings.spreadFactor, enemySettings.spreadFactor),
                targetPosition.z + Random.Range(-enemySettings.spreadFactor, enemySettings.spreadFactor)
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

    private void PlayAudio(List<AudioClip> audioClipList, float volume, bool pitch)
    {
        if (audioClipList.Count == 0)
        {
            Debug.LogError("Error: AudioClip list is empty!");
            return;
        }

        AudioClip randomAudioClip = audioClipList[Random.Range(0, audioClipList.Count)];
        soundController.playAudio(randomAudioClip, volume, pitch);
    }

    public void handleHit()
    {
        if (isHit) return;

        isHit = true;
        StopAllCoroutines();

        StartCoroutine(FadeOut());
        animator.SetBool("isDead", true);
        
        PlayAudio(enemySettings.deathSounds, 1f, true);

        EnemyDeath();
    }

    private void OnPlayerDeath()
    {
        isActive = false;
    }

    public delegate void EnemyDeathCallback(GameObject gameObject, float score);
    public event EnemyDeathCallback onEnemyDeath;
    public void EnemyDeath()
    {
        if (onEnemyDeath != null)
        {
            onEnemyDeath(gameObject, enemySettings.scoreForGettingKilled);
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
        GameController.CurrentGameController.onGameEnd -= OnPlayerDeath;
    }
}
