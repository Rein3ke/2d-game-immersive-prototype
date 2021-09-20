using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 100f;
    [SerializeField]
    LayerMask layerMask;

    ParticleSystem _particleSystem;
    Rigidbody2D _rigidbody2D;
    GameObject _parent;

    public bool CanHitDecoration { get => _canHitDecoration; set => _canHitDecoration = value; }
    private bool _canHitDecoration = true;

    public bool CanHitEnemy { get => _canHitEnemy; set => _canHitEnemy = value; }
    private bool _canHitEnemy = true;

    public bool CanHitInteractables { get => _canHitInteractables; set => _canHitInteractables = value; }
    private bool _canHitInteractables = true;
    
    public bool CanHitForegroundCover { get => _canHitForegroundCover; set => _canHitForegroundCover = value; }
    private bool _canHitForegroundCover = true;

    enum HitObject
    {
        DECORATION, ENEMY, PLAYER, INTERACTABLE_OBJECT, FOREGROUND_COVER
    }

    private void Awake()
    {
        _parent = gameObject.transform.parent.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = _parent.GetComponentInChildren<ParticleSystem>();
        if (_particleSystem == null) Debug.LogWarning("Particle System not found!");

        _rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _rigidbody2D.useFullKinematicContacts = true;

        Destroy(_parent, 4f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer & layerMask) != 0) {
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                    HandleHit(HitObject.ENEMY);
                    break;
                case "Decoration":
                    HandleHit(HitObject.DECORATION);
                    break;
                case "InteractableObject":
                    HandleHit(HitObject.INTERACTABLE_OBJECT);
                    break;
                case "Decoration_Foreground":
                    HandleHit(HitObject.FOREGROUND_COVER);
                    break;
            }
        }
    }

    private void HandleHit(HitObject hitObject)
    {
        switch (hitObject)
        {
            case HitObject.ENEMY:
                if (_canHitEnemy) _particleSystem.Stop();
                break;
            case HitObject.DECORATION:
                if (_canHitDecoration) _particleSystem.Stop();
                break;
            case HitObject.INTERACTABLE_OBJECT:
                if (_canHitInteractables) _particleSystem.Stop();
                break;
            case HitObject.FOREGROUND_COVER:
                if (_canHitForegroundCover)
                {
                    Destroy(_parent);
                    _particleSystem.Stop();
                }
                break;
        }
    }

    internal void MoveToPosition(Vector3 direction, float movementSpeed = 100f)
    {
        StartCoroutine(MoveTo(direction, movementSpeed));
    }

    private IEnumerator MoveTo(Vector3 direction, float movementSpeed)
    {
        while(Level.i.IsGameRunning)
        {
            _parent.transform.position += direction * movementSpeed * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
