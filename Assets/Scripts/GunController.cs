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

    // Start is called before the first frame update
    void Awake()
    {
        if (_i == null) _i = this;
        else Debug.LogError("Error: Too many active gun controllers!");
    }

    private void Start()
    {
        soundController = GameController.CurrentGameController.SoundController;

        GameController.CurrentGameController.InputController.onLeftMousePressed += OnLeftMouseButton;
        GameController.CurrentGameController.InputController.onKeyR += OnRKey;
    }

    private void OnLeftMouseButton()
    {
        if (!Level.i.IsGameRunning) return;
        if (!isGunReady) return;
        if (_playerSettings.playerAmmunition == 0)
        {
            soundController.playAudio(_playerSettings.gunEmptyAudioClip, false);
            return;
        }

        Vector3 mousePosition = Input.mousePosition;

        mousePosition.x += Random.Range(-_playerSettings.playerGunSpreadFactor, _playerSettings.playerGunSpreadFactor);
        mousePosition.y += Random.Range(-_playerSettings.playerGunSpreadFactor, _playerSettings.playerGunSpreadFactor);
        mousePosition.z += Random.Range(-_playerSettings.playerGunSpreadFactor, _playerSettings.playerGunSpreadFactor);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawLine(ray.origin, ray.direction, Color.green, .5f, false);

        soundController.playAudio(_playerSettings.gunShotAudioClip, true);

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 20.0f, layerMask);

        if (hit2D.collider != null)
        {
            RayCastHit(hit2D);
        }

        _playerSettings.playerAmmunition--;
        AmmunitionChange();

        isGunReady = false;

        StartCoroutine(WaitForCooldown(.8f));
    }

    private void OnRKey()
    {
        if (!Level.i.IsGameRunning) return;
        if (_playerSettings.playerAmmunition < _playerSettings.playerMaxAmmunition && !isGunReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isGunReady = false;
        isGunReloading = true;

        while (_playerSettings.playerAmmunition < _playerSettings.playerMaxAmmunition)
        {
            _playerSettings.playerAmmunition++;
            AmmunitionChange();
            soundController.playAudio(_playerSettings.gunReloadAudioClip, false);
            yield return new WaitForSeconds(_playerSettings.playerReloadTime / _playerSettings.playerMaxAmmunition);
        }
        soundController.playAudio(_playerSettings.gunPostReloadAudioClip, false);

        isGunReady = true;
        isGunReloading = false;

        yield return false;
    }

    private IEnumerator WaitForCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isGunReady = true;
    }

    #region Events
    public delegate void RayCastCallback(RaycastHit2D hit);
    public event RayCastCallback onRayCastHit;
    public void RayCastHit(RaycastHit2D hit)
    {
        if (onRayCastHit != null)
        {
            onRayCastHit(hit);
        }
    }

    public event Action onAmmunitionChange;
    public void AmmunitionChange()
    {
        if (onAmmunitionChange != null)
        {
            onAmmunitionChange();
        }
    }
    #endregion

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onLeftMousePressed -= OnLeftMouseButton;
        GameController.CurrentGameController.InputController.onKeyR -= OnRKey;
    }

}
