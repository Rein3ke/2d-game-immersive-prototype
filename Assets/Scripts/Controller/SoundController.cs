using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Controller
{
    /// <summary>
    /// Controller that manages the playback of all sounds.
    /// </summary>
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private float audioPitchFactor = 0.25f;

        /// <summary>
        /// Starts a coroutine that plays a received sound clip.
        /// </summary>
        /// <param name="clip">Audio clip to be played.</param>
        /// <param name="pitch">Should the audio clip be pitched?</param>
        /// <returns>Returns the Audio Clip length in seconds.</returns>
        public float PlayAudio(AudioClip clip, bool pitch)
        {
            float clipLength = clip.length;
            StartCoroutine(PlayAudioClip(clip, 1.0f, pitch, "SFX"));
            return clipLength;
        }

        /// <summary>
        /// Starts a coroutine that plays a received sound clip.
        /// </summary>
        /// <param name="clip">Audio clip to be played.</param>
        /// <param name="volume">Volume of the audio clip (from 0.0f to 1.0f).</param>
        /// <param name="pitch">Should the audio clip be pitched?</param>
        /// <returns>Returns the Audio Clip length in seconds.</returns>
        public float PlayAudio(AudioClip clip, float volume, bool pitch)
        {
            float clipLength = clip.length;
            StartCoroutine(
                PlayAudioClip(
                    clip,
                    Mathf.Clamp(volume, 0.0f, 1.0f),
                    pitch,
                    "SFX"
                )
            );
            return clipLength;
        }

        /// <summary>
        /// Coroutine: Creates an audio source component and takes an audio clip, volume and a pitch boolean for it.
        /// After the audio clip has been played, the component is deleted again.
        /// </summary>
        /// <param name="audioClip">Audio clip to be played.</param>
        /// <param name="volume">Volume of the audio clip (from 0.0f to 1.0f).</param>
        /// <param name="pitch">Should the audio clip be pitched?</param>
        /// <param name="audioMixerGroup">Name of the audio mixer group (as string).</param>
        /// <returns>Nothing</returns>
        private IEnumerator PlayAudioClip(AudioClip audioClip, float volume, bool pitch, string audioMixerGroup)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            
            audioSource.clip = audioClip;
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(audioMixerGroup)[0];
            
            var clipLength = audioClip.length;

            if (pitch)
                audioSource.pitch = 1.0f + Random.Range(-audioPitchFactor, audioPitchFactor);
            else
                audioSource.pitch = 1.0f;

            audioSource.volume = volume;
            // Play the audio
            audioSource.PlayOneShot(audioClip);
            
            yield return new WaitForSeconds(clipLength);
            
            Destroy(audioSource);
            
            yield return null;
        }
    }
}
