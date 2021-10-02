using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Controller
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private float audioPitchFactor = 0.25f;

        public float PlayAudio(AudioClip clip, bool pitch)
        {
            float clipLength = clip.length;
            StartCoroutine(PlayAudioClip(clip, 1.0f, pitch, "SFX"));
            return clipLength;
        }
    
        public float PlayAudio(AudioClip clip, float volume, bool pitch)
        {
            float clipLength = clip.length;
            StartCoroutine(PlayAudioClip(clip, Mathf.Clamp(volume, 0.0f, 1.0f), pitch, "SFX"));
            return clipLength;
        }

        private IEnumerator PlayAudioClip(AudioClip audioClip, float volume, bool pitch, string audioMixerGroup)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(audioMixerGroup)[0];
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
}
