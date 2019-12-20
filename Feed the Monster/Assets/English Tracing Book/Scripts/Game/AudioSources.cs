using UnityEngine;
using System.Collections;
using IndieStudio.EnglishTracingBook.Utility;

/*
 * English Tracing Book Package
 *
 * @license		    Unity Asset Store EULA https://unity3d.com/legal/as_terms
 * @author		    Indie Studio - Baraa Nasser
 * @Website		    https://indiestd.com
 * @Asset Store     https://www.assetstore.unity3d.com/en/#!/publisher/9268
 * @Unity Connect   https://connect.unity.com/u/5822191d090915001dbaf653/column
 * @email		    info@indiestd.com
 *
 */

namespace IndieStudio.EnglishTracingBook.Game
{
    [DisallowMultipleComponent]
    public class AudioSources : MonoBehaviour
    {
        /// <summary>
        /// The audio sources references.
        /// First Audio Souce used for the music
        /// Second Audio Souce used for the sound effects
        /// </summary>
        private AudioSource[] audioSources;

        //AudioClips references

        /// <summary>
        /// Background music
        /// </summary>
        public AudioClip backgroundMusic;

        /// <summary>
        /// Button Click SFX
        /// </summary>
        public AudioClip buttonClickSFX;

        /// <summary>
        /// The bubble sound effect.
        /// </summary>
        public AudioClip bubbleSFX;

        /// <summary>
        /// The completed sound effect.
        /// </summary>
        public AudioClip[] completedSFX;

        /// <summary>
        /// The correct sound effect.
        /// </summary>
        public AudioClip[] correctSFX;

        /// <summary>
        /// The wrong sound effect.
        /// </summary>
        public AudioClip wrongSFX;

        /// <summary>
        /// The locked sound effect.
        /// </summary>
        public AudioClip lockedSFX;

        /// <summary>
        /// Star sound effect.
        /// </summary>
        public AudioClip starSFX;

        /// <summary>
        /// Drop star sound effect
        /// </summary>
        public AudioClip dropStarSFX;

        /// <summary>
        /// This Gameobject defined as a Singleton.
        /// </summary>
        public static AudioSources instance;

        bool reset = true;
        int corsnd;
        // Use this for initialization
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                audioSources = GetComponents<AudioSource>();
                SetUpMuteValues();
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            //Play the background music clip on start
            PlayBackgroundMusic();
        }

        /// <summary>
        /// Set up the mute values for sfx, music audio sources.
        /// </summary>
        private void SetUpMuteValues()
        {
            MusicAudioSource().mute = DataManager.GetMusicMuteValue();
            SFXAudioSource().mute = DataManager.GetSFXMuteValue();
        }

        /// <summary>
        /// Returns the Audio Source of the Music.
        /// </summary>
        /// <returns>The Audio Source of the Music.</returns>
        public AudioSource MusicAudioSource()
        {
            return audioSources[0];
        }


        /// <summary>
        /// Returns the Audio Source of the Sound Effects.
        /// </summary>
        /// <returns>The Audio Source of the Sound Effects.</returns>
        public AudioSource SFXAudioSource()
        {
            return audioSources[1];
        }

        /// <summary>
        /// Play the given SFX clip.
        /// </summary>
        /// <param name="clip">The Clip reference.</param>
        /// <param name="Stop Current Clip">If set to <c>true</c> stop current clip.</param>
        public void PlaySFXClip(AudioClip clip, bool stopCurrentClip)
        {
            if (clip == null)
            {
                return;
            }
            if (stopCurrentClip)
            {
                SFXAudioSource().Stop();
            }
            SFXAudioSource().PlayOneShot(clip);
        }


        /* List of play methods */

        public void PlayButtonClickSFX()
        {
            PlaySFXClip(buttonClickSFX, false);
        }

        public void PlayBackgroundMusic()
        {
            MusicAudioSource().clip = backgroundMusic;
            MusicAudioSource().Play();
        }

        public void PlayBubbleSFX()
        {
            PlaySFXClip(bubbleSFX, false);
        }

        public void PlayCompletedSFX()
        {
            PlaySFXClip(correctSFX[corsnd], false);
            PlaySFXClip(completedSFX[Random.Range(0,completedSFX.Length)], false);
            corsnd = 0;
        }

        public void PlayCorrectSFX()
        {
            PlaySFXClip(correctSFX[corsnd], false);
            corsnd++;
            corsnd = Mathf.Min(corsnd, correctSFX.Length - 1);
        }

        public void PlayWrongSFX()
        {
            PlaySFXClip(wrongSFX, true);
          //  corsnd = 0;
        }

        public void PlayLockedSFX()
        {
            PlaySFXClip(lockedSFX, false);
        }

        public void PlayStarSFX()
        {
            PlaySFXClip(starSFX, false);
        }

        public void PlayDropStarSFX()
        {
            PlaySFXClip(dropStarSFX, false);
        }
    }
}