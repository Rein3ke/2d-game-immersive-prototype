using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IHitable
{
    public static int Count
    {
        get => _count;
        set => _count = value;
    }
    private static int _count = 0;

    public EnemySettings EnemySettings
    {
        get => _enemySettings;
    }

    [SerializeField]
    private EnemySettings _enemySettings;
    [SerializeField]
    private LayerMask _layerMask;

    private GameController _gameController;

    private bool _isHit = false;
    public SpriteRenderer SpriteRenderer
    {
        get => _spriteRenderer;
    }
    private SpriteRenderer _spriteRenderer;
    private SoundController _soundController;
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameController.CurrentGameController;

        // Set Sprite Renderer
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer == null) Debug.LogError("Error: SpriteRenderer not found!");

        _spriteRenderer.sprite = _enemySettings.sprite;
        _spriteRenderer.color = _enemySettings.color;

        _boxCollider2D = GetComponentInChildren<BoxCollider2D>();
        if (_boxCollider2D == null) Debug.LogError("Error: BoxCollider2D not found!");

        _animator = GetComponentInChildren<Animator>();
        if (_animator == null) Debug.LogError("Error: Animator not found!");

        _soundController = _gameController.SoundController;
        if (_soundController == null) Debug.LogError("Error: SoundController not found!");

        EnemySettingsErrorCheck();

        _count++;

        StartCoroutine(EnemyBehaviour());
    }

    private void EnemySettingsErrorCheck()
    {
        if (_enemySettings.switchPositionTime <= 0) Debug.LogWarning("Warning: SwitchPositionTime not set!");
    }

    private IEnumerator EnemyBehaviour()
    {
        // Play Spawn Sound (Random)
        PlayAudio(_enemySettings.spawnSounds, .3f, true);

        while (Level.i.IsGameRunning && !_isHit)
        {
            GameObject[] covers = GameObject.FindGameObjectsWithTag("Position_Cover");
            if (covers.Length > 0)
            {
                int randomCoverIndex = Random.Range(0, covers.Length);
                Vector3 position = covers[randomCoverIndex].transform.position;

                StartCoroutine(GoToPosition(position));
            }
            else
            {
                Debug.LogError("Error: No cover found!");
            }
            yield return new WaitForSeconds(20f);
        }
        yield return null;
    }

    private IEnumerator GoToPosition(Vector3 position)
    {
        Vector3 currentPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            position.x + Random.Range(-1, 1),
            position.y,
            position.z
            );

        FlipSpriteBasedOnDirection(currentPosition, targetPosition);

        float elapsedTime = 0;
        float switchPositionTime = _enemySettings.switchPositionTime;

        StopCoroutine(Shoot());

        _animator.SetBool("isRunning", true);

        // True as long as the required time is not reached.
        while ((elapsedTime < switchPositionTime) && !_isHit)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, (elapsedTime / switchPositionTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _animator.SetBool("isRunning", false);

        StartCoroutine(Shoot());
        yield return null;
    }

    // Flip sprite based on the x value
    private void FlipSpriteBasedOnDirection(Vector3 currentPosition, Vector3 targetPosition)
    {
        Vector3 localDirection = transform.InverseTransformDirection(currentPosition - targetPosition);
        if (localDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
            _boxCollider2D.offset = new Vector2(_boxCollider2D.offset.x * -1, _boxCollider2D.offset.y);
        } else
        {
            _spriteRenderer.flipX = false;
            _boxCollider2D.offset = new Vector2(Mathf.Abs(_boxCollider2D.offset.x), _boxCollider2D.offset.y);
        }
    }

    private IEnumerator FadeOut()
    {
        StopCoroutine(Shoot());

        Color c;
        for (float ft = 1f; ft >= 0; ft -= 0.5f * Time.deltaTime)
        {
            c = _spriteRenderer.color;
            c.a = ft;
            _spriteRenderer.color = c;
            yield return null;
        }

        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator Shoot()
    {
        while(!_isHit && Level.i.IsGameRunning)
        {
            PlayAudio(_enemySettings.shootingSounds, .1f, true);

            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            GameObject hitObject = PerformRayCast(playerGameObject.transform.position, true);

            if (hitObject == null)
            {
                PlayAudio(_enemySettings.hitNothingSounds, .3f, true);
            }
            else
            {
                switch (hitObject.tag)
                {
                    case "Decoration_Foreground":
                        PlayAudio(_enemySettings.hitObjectSounds, 1.0f, true);
                        break;
                    case "Player":
                        PlayAudio(_enemySettings.hitPlayerSounds, 1.0f, true);
                        PlayerHit(_enemySettings.damage);
                        break;
                }
            }

            yield return new WaitForSeconds(_enemySettings.shootingInterval);
        }
        yield return null;
    }

    private GameObject PerformRayCast(Vector3 targetPosition, bool enableAccuracy)
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z);
        
        if (enableAccuracy)
        {
            targetPosition = new Vector3(
                targetPosition.x + Random.Range(-_enemySettings.spreadFactor, _enemySettings.spreadFactor),
                targetPosition.y + Random.Range(-_enemySettings.spreadFactor, _enemySettings.spreadFactor),
                targetPosition.z + Random.Range(-_enemySettings.spreadFactor, _enemySettings.spreadFactor)
                );
        }
        
        Vector3 direction = targetPosition - origin;
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 2f);
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 50f, _layerMask);

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
        _soundController.playAudio(randomAudioClip, volume, pitch);
    }

    public void handleHit()
    {
        if (_isHit) return;

        _isHit = true;
        StopAllCoroutines();

        StartCoroutine(FadeOut());
        _animator.SetBool("isDead", true);
        
        PlayAudio(_enemySettings.deathSounds, 1f, true);

        EnemyDeath();
    }

    #region Events
    public delegate void EnemyDeathCallback(GameObject gameObject, float score);
    public event EnemyDeathCallback onEnemyDeath;
    public void EnemyDeath()
    {
        if (onEnemyDeath != null)
        {
            onEnemyDeath(gameObject, _enemySettings.scoreForGettingKilled);
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
    #endregion

    private void OnDestroy()
    {
        _count--;
    }
}
