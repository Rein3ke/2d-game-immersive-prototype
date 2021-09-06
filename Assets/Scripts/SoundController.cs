using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField]
    private float audioPitchFactor = 0.25f;

    public float playAudio(AudioClip clip, bool pitch)
    {
        float clipLength = clip.length;
        StartCoroutine(PlayAudioClip(clip, pitch));
        return clipLength;
    }

    private IEnumerator PlayAudioClip(AudioClip audioClip, bool pitch)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        float clipLength = audioClip.length;

        if (pitch)
        {
            audioSource.pitch = 1.0f + Random.Range(-audioPitchFactor, audioPitchFactor);
        }
        else
        {
            audioSource.pitch = 1.0f;
        }

        audioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(clipLength);
        Destroy(audioSource);
        yield return null;
    }
}
