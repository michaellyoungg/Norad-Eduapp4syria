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
    public class WinDialog : MonoBehaviour
    {
        /// <summary>
        /// Number of stars for the WinDialog.
        /// </summary>
        private ShapesManager.Shape.StarsNumber starsNumber;

        /// <summary>
        /// Win dialog animator.
        /// </summary>
        public Animator WinDialogAnimator;

        /// <summary>
        /// First star fading animator.
        /// </summary>
        public Animator firstStarFading;

        /// <summary>
        /// Second star fading animator.
        /// </summary>
        public Animator secondStarFading;

        /// <summary>
        /// Third star fading animator.
        /// </summary>
        public Animator thirdStarFading;

        /// <summary>
        /// The level title text.
        /// </summary>
        public Text levelTitle;

        /// <summary>
        /// Whether the dialog is visible or not.
        /// </summary>
        private bool visible;

        // Use this for initialization
        void Start()
        {
            ///Setting up the references
            if (WinDialogAnimator == null)
            {
                WinDialogAnimator = GetComponent<Animator>();
            }

            if (firstStarFading == null)
            {
                firstStarFading = transform.Find("Stars").Find("FirstStarFading").GetComponent<Animator>();
            }

            if (secondStarFading == null)
            {
                secondStarFading = transform.Find("Stars").Find("SecondStarFading").GetComponent<Animator>();
            }

            if (thirdStarFading == null)
            {
                thirdStarFading = transform.Find("Stars").Find("ThirdStarFading").GetComponent<Animator>();
            }

            if (levelTitle == null)
            {
                levelTitle = transform.Find("Level").GetComponent<Text>();
            }
        }

        /// <summary>
        /// When the GameObject becomes visible
        /// </summary>
        void OnEnable()
        {
            //Hide the Win Dialog
            Hide();
        }

        /// <summary>
        /// Show the Win Dialog.
        /// </summary>
        public void Show()
        {
            if (WinDialogAnimator == null)
            {
                return;
            }
            visible = true;
            WinDialogAnimator.SetTrigger("Running");
        }

        /// <summary>
        /// Hide the Win Dialog.
        /// </summary>
        public void Hide()
        {
            visible = false;
            StopAllCoroutines();
            WinDialogAnimator.SetBool("Running", false);
            firstStarFading.SetBool("Running", false);
            secondStarFading.SetBool("Running", false);
            thirdStarFading.SetBool("Running", false);
        }

        /// <summary>
        /// Fade stars Coroutine.
        /// </summary>
        /// <returns>The stars.</returns>
        public IEnumerator FadeStars()
        {
            if (visible)
            {
                starsNumber = Timer.instance.progress.starsNumber;
                float delayBetweenStars = 0.5f;
                if (starsNumber == ShapesManager.Shape.StarsNumber.ONE)
                {//Fade with One Star
                    AudioSources.instance.PlayStarSFX();
                    firstStarFading.SetTrigger("Running");
                }
                else if (starsNumber == ShapesManager.Shape.StarsNumber.TWO)
                {//Fade with Two Stars
                    AudioSources.instance.PlayStarSFX();
                    firstStarFading.SetTrigger("Running");
                    yield return new WaitForSeconds(delayBetweenStars);
                    AudioSources.instance.PlayStarSFX();
                    secondStarFading.SetTrigger("Running");
                }
                else if (starsNumber == ShapesManager.Shape.StarsNumber.THREE)
                {//Fade with Three Stars
                    AudioSources.instance.PlayStarSFX();
                    firstStarFading.SetTrigger("Running");
                    yield return new WaitForSeconds(delayBetweenStars);
                    AudioSources.instance.PlayStarSFX();
                    secondStarFading.SetTrigger("Running");
                    yield return new WaitForSeconds(delayBetweenStars);
                    AudioSources.instance.PlayStarSFX();
                    thirdStarFading.SetTrigger("Running");
                }
            }
            yield return 0;
        }


        /// <summary>
        /// Set the level title.
        /// </summary>
        /// <param name="value">Value.</param>
        public void SetLevelTitle(string value)
        {
            if (string.IsNullOrEmpty(value) || levelTitle == null)
            {
                return;
            }
            levelTitle.text = value;
        }
    }
}