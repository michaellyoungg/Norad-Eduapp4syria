using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
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

namespace IndieStudio.EnglishTracingBook.Utility
{
    [DisallowMultipleComponent]
    public class SceneLoader : MonoBehaviour
    {

        /// <summary>
        /// The canvas group.
        /// </summary>
        public Image canvasGroup;

        public Color transcolor, nottrans;

        /// <summary>
        /// Loading image reference
        /// </summary>
        public Image loadingImage;
 
        /// <summary>
        /// This Gameobject defined as a Singleton.
        /// </summary>
        public static SceneLoader instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

           

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            if (this.GetInstanceID() == instance.GetInstanceID())
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }
        }

        /// <summary>
        /// On Load the scene.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        
            if (canvasGroup != null)
            {
                canvasGroup.color = transcolor;
                //StartCoroutine(CanvasFade(FadeType.FADE_OUT));
            }
        }

        /// <summary>
        /// Loads scene coroutine.
        /// </summary>
        public IEnumerator LoadScene(string sceneName)
        {
            gameObject.SetActive(true);

            yield return 0;

            if (!string.IsNullOrEmpty(sceneName))
            {

                if (canvasGroup != null)
                {
                    canvasGroup.color = transcolor;
                 //   yield return StartCoroutine(CanvasFade(FadeType.FADE_IN));
                }
                if (loadingImage!=null)
                 loadingImage.gameObject.SetActive(true);
                SceneManager.LoadScene(sceneName);
            }
        }

        /// <summary>
        /// Fade in/out the canvas.
        /// </summary>
        public IEnumerator CanvasFade()
        {
           

            float delay = 0.03f;
            float alphaOffset = 0.1f;

            canvasGroup.raycastTarget = true;

            while (canvasGroup.color != Color.black)
            {
                yield return new WaitForSeconds(delay);
                canvasGroup.color = Color.Lerp(canvasGroup.color, Color.black, .1f);
            }

        

          
        }

        public enum FadeType
        {
            FADE_IN,
            FADE_OUT
        }
    }
}