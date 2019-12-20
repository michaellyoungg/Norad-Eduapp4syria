using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// Text Component
        /// </summary>
        public Text uiText;

        /// <summary>
        /// The time in seconds.
        /// </summary>
        [HideInInspector]
        public int timeInSeconds;

        /// <summary>
        /// Whether the timer is paused or not.
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// The progress reference.
        /// </summary>
        public Progress progress;

        /// <summary>
        /// Whether the Timer is running
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// The time counter.
        /// </summary>
        private float timeCounter;

        /// <summary>
        /// The sleep time.
        /// </summary>
        private float sleepTime;

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static Timer instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {

            if (uiText == null)
            {
                uiText = GetComponent<Text>();
            }

            ///Run the Timer
            Run();
        }

        /// <summary>
        /// Run the Timer.
        /// </summary>
        public void Run()
        {
            if (!isRunning)
            {
                isPaused = false;
                timeCounter = 0;
                sleepTime = 0.01f;
                isRunning = true;
                timeInSeconds = 0;

                if (progress != null)
                {
                    progress.ResetDropFlags();
                }
                InvokeRepeating("Wait", 0, sleepTime);
            }
        }

        /// <summary>
        /// Pause the Timer.
        /// </summary>
        public void Pause()
        {
            isPaused = false;
        }

        /// <summary>
        /// Resume the Timer.
        /// </summary>
        public void Resume()
        {
            isPaused = true;
        }

        /// <summary>
        /// Stop the Timer.
        /// </summary>
        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                CancelInvoke();
            }
        }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        public void Reset()
        {
            Stop();
            Run();
        }

        /// <summary>
        /// Wait.
        /// </summary>
        private void Wait()
        {
            if (isPaused)
            {
                return;
            }
            timeCounter += sleepTime;
            timeInSeconds = (int)timeCounter;
            ApplyTime();
            if (progress != null)
                progress.SetProgress(timeCounter);
        }

        /// <summary>
        /// Applies the time into TextMesh Component.
        /// </summary>
        private void ApplyTime()
        {
            if (uiText == null)
            {
                return;
            }
            //	int mins = timeInSeconds / 60;
            //	int seconds = timeInSeconds % 60;

            //	uiText.text = "Time : " + GetNumberWithZeroFormat (mins) + ":" + GetNumberWithZeroFormat (seconds);
            uiText.text = timeInSeconds.ToString();
        }

        /// <summary>
        /// Get the number with zero format.
        /// </summary>
        /// <returns>The number with zero format.</returns>
        /// <param name="number">Ineger Number.</param>
        public static string GetNumberWithZeroFormat(int number)
        {
            string strNumber = "";
            if (number < 10)
            {
                strNumber += "0";
            }
            strNumber += number;

            return strNumber;
        }
    }
}