using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public class Logo : MonoBehaviour
    {
        /// <summary>
        /// The sleep time.
        /// </summary>
        public float sleepTime = 5;

        /// <summary>
        /// The name of the scene to load.
        /// </summary>
        public string nextSceneName;

        // Use this for initialization
        void Start()
        {
            Invoke("LoadScene", sleepTime);
        }

        private void LoadScene()
        {
            if (string.IsNullOrEmpty(nextSceneName))
            {
                return;
            }
            SceneManager.LoadScene(nextSceneName);
        }
    }
}