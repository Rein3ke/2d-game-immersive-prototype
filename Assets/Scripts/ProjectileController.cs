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
                    _particleSystem.Stop();
                    break;
                case "Decoration":
                    _particleSystem.Stop();
                    break;
                case "InteractableObject":
                    _particleSystem.Stop();
                    break;
            }
        }
    }

    internal void MoveToPosition(Vector3 direction)
    {
        StartCoroutine(MoveTo(direction));
    }

    private IEnumerator MoveTo(Vector3 direction)
    {
        while(Level.i.IsGameRunning)
        {
            _parent.transform.position += direction * _speed * Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
