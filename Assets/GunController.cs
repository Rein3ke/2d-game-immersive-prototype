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

        GameController.CurrentGameController.InputController.onLeftMousePressed += handleLeftMouseButton;
    }

    private void handleLeftMouseButton()
    {
        if (!isGunReady) return;

        Vector3 mousePosition = Input.mousePosition;

        mousePosition.x += Random.Range(-spreadFactor, spreadFactor);
        mousePosition.y += Random.Range(-spreadFactor, spreadFactor);
        mousePosition.z += Random.Range(-spreadFactor, spreadFactor);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawLine(ray.origin, ray.direction, Color.green, .5f, false);

        audioSource.pitch = 1.0f + Random.Range(-audioPitchFactor, audioPitchFactor);
        audioSource.Play();

        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray, 20.0f, layerMask);

        if (hit2D.collider != null)
        {
            RayCastHit(hit2D);
        }

        isGunReady = false;

        StartCoroutine(WaitForCooldown());
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

    private void OnDisable()
    {
        GameController.CurrentGameController.InputController.onLeftMousePressed -= handleLeftMouseButton;
    }

    private IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(.5f);
        isGunReady = true;
    }
}
