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
    public class Dialog : MonoBehaviour
    {
        /// <summary>
        /// The animator of the dialog.
        /// </summary>
        public Animator animator;

        /// <summary>
        /// The visible flag.
        /// </summary>
        [HideInInspector]
        public bool visible;

        void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        /// <summary>
        /// Show the dialog.
        /// </summary>
        public void Show(bool playClickSFX)
        {
            SetActiveTrue();

            BlackArea.instance.Show();
           // animator.SetBool("Off", false);
            //animator.SetTrigger("On");
            visible = true;
        }

        /// <summary>
        /// Hide the dialog.
        /// </summary>
        public void Hide(bool playClickSFX)
        {
            if (playClickSFX)
                AudioSources.instance.PlayButtonClickSFX();

            BlackArea.instance.Hide();
            //animator.SetBool("On", false);
            //animator.SetTrigger("Off");
            visible = false;
           // AdsManager.instance.HideAdvertisment();
           // GameObject.FindObjectOfType<SceneStartup>().ShowAd();
        }

        /// <summary>
        /// Set gameobject active false.
        /// </summary>
        public void SetActiveFalse()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Set gameobject active true.
        /// </summary>
        public void SetActiveTrue()
        {
            gameObject.SetActive(true);
        }
    }
}