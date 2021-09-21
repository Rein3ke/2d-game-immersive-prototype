using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleController : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        Destroy(transform.parent.gameObject);
    }
}
