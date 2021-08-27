using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientController : MonoBehaviour
{
    public Material gradientMaterial;

    [Range(0.0f, 1.0f)]
    public float gradient = 0.0f;

    private void Update()
    {
        gradientMaterial.SetFloat("_GradientValue", gradient);
    }
}
