using UnityEngine;

/// <summary>
/// A script that responds to the completion of a particle animation and then deletes itself (including parent object).
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class DestroyParticleController : MonoBehaviour
{
    /// <summary>
    /// Destroy the parent when ParticleSystem stops.
    /// </summary>
    private void OnParticleSystemStopped()
    {
        Destroy(transform.parent.gameObject);
    }
}
