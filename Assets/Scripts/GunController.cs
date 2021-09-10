using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;
    
    [SerializeField]
    private PlayerSettings playerSettings;

    private static GunController _instance;
    public static GunController Instance
    {
        get => _instance;
    }

    private SoundController soundController;
    private bool isGunReady = true;
    private bool isActive = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Debug.LogError("Error: Too many active gun controllers!");
    }

    private void Start()
    {
        soundController = GameController.CurrentGameController.SoundController;

        GameController.CurrentGameController.InputController.onLeftMousePressed += OnLeftMouseButton;
        GameController.CurrentGameController.InputController.onKeyR += OnRKey;
        GameController.CurrentGameController.onGameEnd += OnGameEnd;
        GameController.CurrentGameController.onGameWon += OnGameEnd;
    }

    private void OnLeftMouseButton()
    {
        if (!isActive) return;
        if (!isGunReady) return;
        if (playerSettings.playerAmmunition == 0)
        {
            soundController.playAudio(playerSettings.gunEmptyAudioClip, false);
            return;
        }

        Vector3 mousePosition = Input.mousePosition;

        mousePosition.x += Random.Range(-playerSettings.playerGunSpreadFactor, playerSettings.playerGunSpreadFactor);
        mousePosition.y += Random.Range(-playerSettings.playerGunSpreadFactor, playerSettings.playerGunSpreadFactor);
        mousePosition.z += Random.Range(-playerSettings.playerGunSpreadFactor, playerSettings.playerGunSpreadFactor);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawLine(ray.origin, ray.direction, Color.green, .5f, false);

        soundController.playAudio(playerSettings.gunShotAudioClip, true);

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 20.0f, layerMask);

        if (hit2D.collider != null)
        {
            RayCastHit(hit2D);
        }

        playerSettings.playerAmmunition--;
        AmmunitionChange();

        isGunReady = false;

        StartCoroutine(WaitForCooldown(.8f));
    }

    private void OnRKey()
    {
        if (!isActive) return;
        if (playerSettings.playerAmmunition < playerSettings.playerMaxAmmunition)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isGunReady = false;
        while(playerSettings.playerAmmunition < playerSettings.playerMaxAmmunition)
        {
            playerSettings.playerAmmunition++;
            AmmunitionChange();
            soundController.playAudio(playerSettings.gunReloadAudioClip, false);
            yield return new WaitForSeconds(playerSettings.playerReloadTime / playerSettings.playerMaxAmmunition);
        }
        soundController.playAudio(playerSettings.gunPostReloadAudioClip, false);
        isGunReady = true;
        yield return false;
    }

    private IEnumerator WaitForCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isGunReady = true;
    }

    private void OnGameEnd()
    {
        isActive = false;
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
        GameController.CurrentGameController.onGameEnd -= OnGameEnd;
        GameController.CurrentGameController.onGameWon -= OnGameEnd;
    }

}
