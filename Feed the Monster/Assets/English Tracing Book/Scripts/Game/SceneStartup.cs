using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
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
    public class SceneStartup : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
        //    ShowAd();
        }

      

        void OnDestroy()
        {
          //  AdsManager.instance.HideAdvertisment();
        }
    }
}