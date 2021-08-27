using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurAnimated : MonoBehaviour
{
    [SerializeField] private Material material;

    private float blurAmount;
    private bool blurActive;

    // Start is called before the first frame update
    void Start()
    {
        blurAmount = 0.9f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            blurActive = !blurActive;
        }

        float blurSpeed = 10f;
        if(blurActive)
        {
            blurAmount += blurSpeed * Time.deltaTime;
        } else
        {
            blurAmount -= blurSpeed * Time.deltaTime;
        }

        blurAmount = Mathf.Clamp01(blurAmount);
        material.SetFloat("_BlurAmount", blurAmount);
    }
}
