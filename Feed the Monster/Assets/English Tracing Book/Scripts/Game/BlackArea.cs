using UnityEngine;
using System.Collections;

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
    public class BlackArea : MonoBehaviour
    {
        /// <summary>
        /// Black area animator.
        /// </summary>
        private Animator blackAreaAnimator;

        /// <summary>
        /// A static instance of this class.
        /// </summary>
        public static BlackArea instance;

        // Use this for initialization
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            if (blackAreaAnimator == null)
            {
                blackAreaAnimator = GetComponent<Animator>();
            }

            SetActiveFalse();
        }

        /// <summary>
        /// Show the Black Area
        /// </summary>
        public void Show()
        {
            if (blackAreaAnimator == null)
            {
                return;
            }

            SetActiveTrue();

           // blackAreaAnimator.SetTrigger("Running");
        }

        /// <summary>
        /// Hide the Black Area
        /// </summary>
        public void Hide()
        {
            
            SetActiveFalse();
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