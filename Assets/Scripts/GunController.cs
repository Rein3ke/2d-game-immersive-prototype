using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    public PlayerSettings PlayerSettings { get; set; }

    public static GunController I { get; private set; }

    private SoundController soundController;
    private bool _isGunReady = true;
    private bool _isGunReloading;
    private bool _isBehindCover;
    private Camera _mainCamera;

    // Start is called before the first frame update
    void Awake()
    {
        if (I == null) I = this;
        else Debug.LogError("Error: Too many active gun controllers!");
    }

    /// <summary>
    /// Standard unity method. Set all necessary fields and subscribe to events from the input controller.
    /// </summary>
    private void Start()
    {
        _mainCamera = Camera.main;
        
        soundController = GameController.Instance.SoundController;

        // Subscribe to events
        GameController.Instance.InputController.onLeftMouseDown += InputController_OnLeftMouseDown;
        GameController.Instance.InputController.onKeyRDown += InputController_OnKeyRDown;
        GameController.Instance.InputController.onSpaceDown += InputController_OnSpaceDown;
        GameController.Instance.InputController.onSpaceUp += InputController_OnSpaceUp;
    }

    /// <summary>
    /// Coroutine: Gradually counts up the current ammunition to the maximum value.
    /// </summary>
    /// <returns>Nothing</returns>
    private IEnumerator Reload()
    {
        _isGunReady = false;
        _isGunReloading = true;

        while (PlayerSettings.PlayerAmmunition < PlayerSettings.playerMaxAmmunition)
        {
            PlayerSettings.PlayerAmmunition++;
            
            AmmunitionChange();
            
            soundController.PlayAudio(PlayerSettings.gunReloadAudioClip, false);
            
            yield return new WaitForSeconds(PlayerSettings.playerReloadTime / PlayerSettings.playerMaxAmmunition);
        }
        soundController.PlayAudio(PlayerSettings.gunPostReloadAudioClip, false);

        _isGunReady = true;
        _isGunReloading = false;

        yield return false;
    }

    /// <summary>
    /// Coroutine: Sets the status of isGunReady to true after a specified time.
    /// </summary>
    /// <param name="cooldown">Duration in seconds</param>
    /// <returns>Nothing</returns>
    private IEnumerator WaitForCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        
        _isGunReady = true;
    }

    #region Event Handling
    /// <summary>
    /// Perform a raycast and execute handleHit() function of the hit object.
    /// </summary>
    private void InputController_OnLeftMouseDown()
    {
        if (!Level.i.IsGameRunning || _isBehindCover || !_isGunReady) return;
        if (PlayerSettings.PlayerAmmunition == 0)
        {
            soundController.PlayAudio(PlayerSettings.gunEmptyAudioClip, false);
            return;
        }

        // Determines the mouse position and changes it on all axis depending on the spread factor
        var mousePosition = Input.mousePosition;

        mousePosition.x += Random.Range(-PlayerSettings.playerGunSpreadFactor, PlayerSettings.playerGunSpreadFactor);
        mousePosition.y += Random.Range(-PlayerSettings.playerGunSpreadFactor, PlayerSettings.playerGunSpreadFactor);
        mousePosition.z += Random.Range(-PlayerSettings.playerGunSpreadFactor, PlayerSettings.playerGunSpreadFactor);

        var ray = _mainCamera.ScreenPointToRay(mousePosition);
        
        // Draw a line from ray origin to its direction
        Debug.DrawLine(ray.origin, ray.direction, Color.green, .5f, false);

        // Play shot sound
        soundController.PlayAudio(PlayerSettings.gunShotAudioClip, true);

        // Perform raycast
        var hit2D = Physics2D.GetRayIntersection(ray, 20.0f, layerMask);

        // If hit object has a collider, continue
        var hit2DCollider = hit2D.collider;
        if (hit2DCollider != null)
        {
            var hitAble = hit2DCollider.GetComponent<IHitable>();
            hitAble?.handleHit();
        }

        // Load the projectile prefab from resource folder and run its MoveToPosition() method.
        var projectile = Instantiate(Resources.Load("Projectile") as GameObject, transform.position, Quaternion.identity);
        if (projectile != null)
        {
            var projectileController = projectile.GetComponentInChildren<ProjectileController>();
            projectileController.CanHitForegroundCover = false;
            projectileController.MoveToPosition(ray.direction);
        }

        // Reduce ammunition
        PlayerSettings.PlayerAmmunition--;
        
        // Call event
        AmmunitionChange();

        _isGunReady = false;

        StartCoroutine(WaitForCooldown(.8f));
    }
    
    /// <summary>
    /// Set isBehindCover boolean to false if Space was left.
    /// </summary>
    private void InputController_OnSpaceUp()
    {
        _isBehindCover = false;
    }

    /// <summary>
    /// Set isBehindCover boolean to true if Space was pressed.
    /// </summary>
    private void InputController_OnSpaceDown()
    {
        _isBehindCover = true;
    }

    /// <summary>
    /// Start the reload coroutine when the R key is pressed.
    /// </summary>
    private void InputController_OnKeyRDown()
    {
        if (!Level.i.IsGameRunning) return;
        
        if (PlayerSettings.PlayerAmmunition < PlayerSettings.playerMaxAmmunition && !_isGunReloading)
        {
            StartCoroutine(Reload());
        }
    }
    #endregion

    #region Events
    public event Action onAmmunitionChange;
    /// <summary>
    /// Invokes the event when the ammunition value has changed.
    /// </summary>
    private void AmmunitionChange()
    {
        onAmmunitionChange?.Invoke();
    }
    #endregion

    /// <summary>
    /// Unsubscribe from all events if destroyed.
    /// </summary>
    private void OnDestroy()
    {
        GameController.Instance.InputController.onLeftMouseDown -= InputController_OnLeftMouseDown;
        GameController.Instance.InputController.onKeyRDown -= InputController_OnKeyRDown;
        GameController.Instance.InputController.onSpaceDown -= InputController_OnSpaceDown;
        GameController.Instance.InputController.onSpaceUp -= InputController_OnSpaceUp;
    }

}
