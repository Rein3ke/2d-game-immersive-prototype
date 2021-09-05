using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField]
    private float audioPitchFactor = 0.25f;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public float playAudio(AudioClip clip, bool pitch)
    {
        float clipLength = clip.length;

        if (pitch)
        {
            audioSource.pitch = 1.0f + Random.Range(-audioPitchFactor, audioPitchFactor);
        }
        else
        {
            audioSource.pitch = 1.0f;
        }

        audioSource.clip = clip;
        audioSource.PlayOneShot(clip);
        return clipLength;
    }
}
