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
        StartCoroutine(PlayAudioClip(clip, 1.0f, pitch));
        return clipLength;
    }
    public float playAudio(AudioClip clip, float volume, bool pitch)
    {
        float clipLength = clip.length;
        StartCoroutine(PlayAudioClip(clip, Mathf.Clamp(volume, 0.0f, 1.0f), pitch));
        return clipLength;
    }

    private IEnumerator PlayAudioClip(AudioClip audioClip, float volume, bool pitch)
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

        audioSource.volume = volume; 
        audioSource.PlayOneShot(audioClip);
        yield return new WaitForSeconds(clipLength);
        Destroy(audioSource);
        yield return null;
    }
}
