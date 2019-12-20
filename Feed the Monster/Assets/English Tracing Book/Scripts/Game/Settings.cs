using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using IndieStudio.EnglishTracingBook.Utility;
using System.Collections.Generic;

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
    public class Settings : MonoBehaviour
    {
        /// <summary>
        /// The music slider reference.
        /// </summary>
        public Slider musicSlider;

        /// <summary>
        /// The sfx slider reference.
        /// </summary>
        public Slider sfxSlider;

        /// <summary>
        /// Tracing mode button component
        /// </summary>
        public Button tracingModeButton;

        /// <summary>
        /// Sprites of line/fill tracing modes
        /// </summary>
        public Sprite tracingModeLine, tracingModeFill;

        /// <summary>
        /// Whether the vibration is enabled or not.
        /// </summary>
        public static bool vibrationEnabled;

        /// <summary>
        /// The value of the tracing mode
        /// </summary>
        private ShapesManager.TracingMode tracingMode;

        // Use this for initialization
        void Start()
        {
            SetMusicValue(AudioSources.instance.MusicAudioSource().mute);
            SetSFXValue(AudioSources.instance.SFXAudioSource().mute);

            tracingMode = DataManager.GetTracingModeValue() == 0 ? ShapesManager.TracingMode.FILL : ShapesManager.TracingMode.LINE;
            SetTracingModeValue();
        }

        /// <summary>
        /// On music slider change event.
        /// </summary>
        public void OnMusicSliderChange()
        {
            SetMusicValue(musicSlider.value == 1 ? false : true);
        }

        /// <summary>
        /// On sfx slider change event.
        /// </summary>
        public void OnSFXSliderChange()
        {
            SetSFXValue(sfxSlider.value == 1 ? false : true);
        }

        /// <summary>
        /// Toggles the tracing mode
        /// </summary>
        public void ToggleTracinggModeChange()
        {
            //toggle the int value
            if (tracingMode == ShapesManager.TracingMode.FILL)
            {
                tracingMode = ShapesManager.TracingMode.LINE;
            }
            else
            {
                tracingMode = ShapesManager.TracingMode.FILL;
            }

            SetTracingModeValue();
            DataManager.SaveTracingModeValue(tracingMode == ShapesManager.TracingMode.FILL ? 0 : 1);

            foreach (KeyValuePair<string, ShapesManager> shapesManager in ShapesManager.shapesManagers)
            {
                shapesManager.Value.tracingMode = tracingMode;
            }
        }

        /// <summary>
        /// Set up the music value.
        /// </summary>
        /// <param name="mute">whether to mute or not.</param>
        private void SetMusicValue(bool mute)
        {
            AudioSources.instance.MusicAudioSource().mute = mute;
            musicSlider.value = (mute == true ? 0 : 1);
            DataManager.SaveMusicMuteValue(mute == true ? 1 : 0);
        }

        /// <summary>
        /// Set up the SFX value.
        /// </summary>
        /// <param name="value">Value.</param>
        private void SetSFXValue(bool mute)
        {
            AudioSources.instance.SFXAudioSource().mute = mute;
            sfxSlider.value = (mute == true ? 0 : 1);
            DataManager.SaveSFXMuteValue(mute == true ? 1 : 0);
        }

        /// <summary>
        /// Set up the Tracing Mode value.
        /// </summary>
        private void SetTracingModeValue()
        {
            if (tracingMode == ShapesManager.TracingMode.FILL)
            {//Fill
                tracingModeButton.GetComponent<Image>().sprite = tracingModeFill;
            }
            else
            {//Line
                tracingModeButton.GetComponent<Image>().sprite = tracingModeLine;
            }
        }
    }
}
