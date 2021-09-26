using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    public PlayerSettings PlayerSettings
    {
        set => _playerSettings = value;
    }
    private PlayerSettings _playerSettings;

    public static GunController i
    {
        get => _i;
    }
    private static GunController _i;

    private SoundController soundController;
    private bool isGunReady = true;
    private bool isGunReloading = false;
    private bool _isBehindCover = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (_i == null) _i = this;
        else Debug.LogError("Error: Too many active gun controllers!");
    }

    private void Start()
    {
        soundController = GameController.Instance.SoundController;

        GameController.Instance.InputController.onLeftMouseDown += OnLeftMouseButton;
        GameController.Instance.InputController.onKeyRDown += OnRKey;
        GameController.Instance.InputController.onSpacebarDown += OnSpacebarDown;
        GameController.Instance.InputController.onSpacebarUp += OnSpacebarUp;

    }

    private IEnumerator Reload()
    {
        isGunReady = false;
        isGunReloading = true;

        while (_playerSettings.PlayerAmmunition < _playerSettings.playerMaxAmmunition)
        {
            _playerSettings.PlayerAmmunition++;
            AmmunitionChange();
            soundController.PlayAudio(_playerSettings.gunReloadAudioClip, false);
            yield return new WaitForSeconds(_playerSettings.playerReloadTime / _playerSettings.playerMaxAmmunition);
        }
        soundController.PlayAudio(_playerSettings.gunPostReloadAudioClip, false);

        isGunReady = true;
        isGunReloading = false;

        yield return false;
    }

    private IEnumerator WaitForCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isGunReady = true;
    }

    #region Event Handling
    private void OnLeftMouseButton()
    {
        if (!Level.i.IsGameRunning || _isBehindCover || !isGunReady) return;
        if (_playerSettings.PlayerAmmunition == 0)
        {
            soundController.PlayAudio(_playerSettings.gunEmptyAudioClip, false);
            return;
        }

        Vector3 mousePosition = Input.mousePosition;

        mousePosition.x += Random.Range(-_playerSettings.playerGunSpreadFactor, _playerSettings.playerGunSpreadFactor);
        mousePosition.y += Random.Range(-_playerSettings.playerGunSpreadFactor, _playerSettings.playerGunSpreadFactor);
        mousePosition.z += Random.Range(-_playerSettings.playerGunSpreadFactor, _playerSettings.playerGunSpreadFactor);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawLine(ray.origin, ray.direction, Color.green, .5f, false);

        soundController.PlayAudio(_playerSettings.gunShotAudioClip, true);

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 20.0f, layerMask);

        Collider2D collider = hit2D.collider;
        IHitable hitable = collider?.GetComponent<IHitable>();
        if (hitable != null)
        {
            hitable.handleHit();
        }

        GameObject projectile = Instantiate(Resources.Load("Projectile") as GameObject, transform.position, Quaternion.identity);
        if (projectile != null)
        {
            ProjectileController projectileController = projectile.GetComponentInChildren<ProjectileController>();
            projectileController.CanHitForegroundCover = false;
            projectileController.MoveToPosition(ray.direction);
        }

        _playerSettings.PlayerAmmunition--;
        AmmunitionChange();

        isGunReady = false;

        StartCoroutine(WaitForCooldown(.8f));
    }

    private void OnSpacebarUp()
    {
        _isBehindCover = false;
    }

    private void OnSpacebarDown()
    {
        _isBehindCover = true;
    }

    private void OnRKey()
    {
        if (!Level.i.IsGameRunning) return;
        if (_playerSettings.PlayerAmmunition < _playerSettings.playerMaxAmmunition && !isGunReloading)
        {
            StartCoroutine(Reload());
        }
    }
    #endregion

    #region Events
    public event Action onAmmunitionChange;
    public void AmmunitionChange()
    {
        if (onAmmunitionChange != null)
        {
            onAmmunitionChange();
        }
    }
    #endregion

    private void OnDestroy()
    {
        GameController.Instance.InputController.onLeftMouseDown -= OnLeftMouseButton;
        GameController.Instance.InputController.onKeyRDown -= OnRKey;
        GameController.Instance.InputController.onSpacebarDown -= OnSpacebarDown;
        GameController.Instance.InputController.onSpacebarUp -= OnSpacebarUp;
    }

}
