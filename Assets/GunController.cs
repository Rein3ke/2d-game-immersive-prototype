using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private float spreadFactor = 40f;
    [SerializeField]
    private float audioPitchFactor = 0.25f;
    [SerializeField]
    private AudioClip gunShotAudioClip;
    [SerializeField]
    private AudioClip gunRealoadAudioClip;
    [SerializeField]
    private int maxAmmo = 8;

    private int _ammo;
    public int Ammo
    {
        get => _ammo;
        set {
            _ammo = Mathf.Clamp(value, 0, maxAmmo);
            AmmunitionChange(_ammo);
        }
    }

    private static GunController _instance;
    public static GunController Instance
    {
        get => _instance;
    }

    private AudioSource audioSource;
    private bool isGunReady = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null) _instance = this;
        else Debug.LogError("Too many gun controllers!");
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Ammo = maxAmmo;

        GameController.CurrentGameController.InputController.onLeftMousePressed += handleLeftMouseButton;
        GameController.CurrentGameController.InputController.onKeyR += handleRKey;
    }

    private void handleLeftMouseButton()
    {
        if (!isGunReady || Ammo == 0) return;
        Debug.Log(Ammo);

        Vector3 mousePosition = Input.mousePosition;

        mousePosition.x += Random.Range(-spreadFactor, spreadFactor);
        mousePosition.y += Random.Range(-spreadFactor, spreadFactor);
        mousePosition.z += Random.Range(-spreadFactor, spreadFactor);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawLine(ray.origin, ray.direction, Color.green, .5f, false);

        playAudioClip(gunShotAudioClip, true);

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 20.0f, layerMask);

        if (hit2D.collider != null)
        {
            RayCastHit(hit2D);
        }

        Ammo--;

        isGunReady = false;

        StartCoroutine(WaitForCooldown(.8f));
    }

    private float playAudioClip(AudioClip clip, bool pitch)
    {
        float clipLength = clip.length;

        if (pitch)
        {
            audioSource.pitch = 1.0f + Random.Range(-audioPitchFactor, audioPitchFactor);
        } else
        {
            audioSource.pitch = 1.0f;
        }

        audioSource.clip = clip;

        audioSource.Play();
        return clipLength;
    }

    private void handleRKey()
    {
        if (Ammo < maxAmmo)
        {
            isGunReady = false;
            Ammo = maxAmmo;
            StartCoroutine(WaitForCooldown(playAudioClip(gunRealoadAudioClip, false)));
        }
    }

    public delegate void RayCastCallback(RaycastHit2D hit);
    public event RayCastCallback onRayCastHit;
    public void RayCastHit(RaycastHit2D hit)
    {
        if (onRayCastHit != null)
        {
            onRayCastHit(hit);
        }
    }

    public delegate void AmmunitionChangeCallback(int ammo);
    public event AmmunitionChangeCallback onAmmunitionChange;
    public void AmmunitionChange(int ammo)
    {
        if (onAmmunitionChange != null)
        {
            onAmmunitionChange(ammo);
        }
    }

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onLeftMousePressed -= handleLeftMouseButton;
        GameController.CurrentGameController.InputController.onKeyR -= handleRKey;
    }

    private IEnumerator WaitForCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isGunReady = true;
    }
}
