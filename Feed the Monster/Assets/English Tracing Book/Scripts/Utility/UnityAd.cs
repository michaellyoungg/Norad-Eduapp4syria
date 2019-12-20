using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

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
    public class UnityAd : MonoBehaviour
    {
        /// <summary>
        /// The android game id.
        /// </summary>
        public string androidGameID;

        /// <summary>
        /// The ios game id.
        /// </summary>
        public string iosGameID;

        /// <summary>
        /// Enable test mode or not.
        /// </summary>
        public bool testMode;

        /// <summary>
        /// On show unity ads event.
        /// </summary>
        private UnityEvent onShowAdsEvent;

        // Use this for initialization
        void Start()
        {
            //initialize unity ad
            #if UNITY_ADS
                #if UNITY_ANDROID
				        Advertisement.Initialize (androidGameID, testMode);
                #elif UNITY_IPHONE
				        Advertisement.Initialize(iosGameID, testMode);
                #endif
            #endif
        }

        /// <summary>
        /// Show the unity ad.
        /// </summary>
        public void ShowUnityAd(UnityEvent onShowAdsEvent)
        {
            this.onShowAdsEvent = onShowAdsEvent;
            StartCoroutine("UnityAdCoroutine");
        }

        /// <summary>
        /// Unity ad coroutine.
        /// </summary>
        /// <returns>The ad coroutine.</returns>
        private IEnumerator UnityAdCoroutine()
        {
            #if UNITY_ADS
		        while (!Advertisement.IsReady())
		        {
			        yield return new WaitForSeconds(0.1f);
		        }

		        var options = new ShowOptions { resultCallback = HandleShowResult };

		        Advertisement.Show(options);
            #else
                yield return null;
            #endif
        }

        /// <summary>
        /// Handle the show result.
        /// </summary>
        /// <param name="result">Result.</param>
        #if UNITY_ADS
	        private void HandleShowResult (ShowResult result){

		        if (result == ShowResult.Finished) {
			        if(onShowAdsEvent!=null)
				        onShowAdsEvent.Invoke ();
		        } else if (result == ShowResult.Skipped) {
			        if(onShowAdsEvent!=null)
				        onShowAdsEvent.Invoke ();
		        } else if (result == ShowResult.Failed) {
			        Debug.Log ("Failed to show unity ads");
		        }
	        }
        #endif
    }
}