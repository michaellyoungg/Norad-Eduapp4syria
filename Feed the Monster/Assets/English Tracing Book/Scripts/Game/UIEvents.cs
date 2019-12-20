using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
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
    public class UIEvents : MonoBehaviour
    {
        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static UIEvents instance;

        //Set of dialogs
        private Dialog renewHelpBoosterDialog;

        /// <summary>
        /// A Unity event.
        /// </summary>
        private UnityEvent unityEvent = new UnityEvent();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            GameObject temp;

            temp = GameObject.Find("RenewHelpBoosterDialog");
            if (temp != null)
            {
                renewHelpBoosterDialog = temp.GetComponent<Dialog>();
            }
        }

        public void AlbumShapeEvent(TableShape tableShape)
        {
            if (tableShape == null)
            {
                return;
            }

            ShapesManager shapesManager = ShapesManager.GetCurrentShapesManager();

            if (shapesManager == null)
            {
                return;
            }

            if (shapesManager.shapes[tableShape.ID].isLocked && !shapesManager.testMode)
            {
                return;
            }

            ShapesManager.Shape.selectedShapeID = tableShape.ID;
            //LoadGameScene();
        }

        public void PointerButtonEvent(Pointer pointer)
        {
            if (pointer == null)
            {
                return;
            }
            if (pointer.group != null)
            {
                ScrollSlider scrollSlider = GameObject.FindObjectOfType(typeof(ScrollSlider)) as ScrollSlider;
                if (scrollSlider != null)
                {
                    scrollSlider.DisableCurrentPointer();
                    FindObjectOfType<ScrollSlider>().currentGroupIndex = pointer.group.Index;
                    scrollSlider.GoToCurrentGroup();
                }
            }
        }

      

        public void NextClickEvent()
        {
            GameManager.instance.NextShape();
        }

        public void PreviousClickEvent()
        {
            GameManager.instance.PreviousShape();
        }

        public void SpeechClickEvent()
        {
            GameManager.instance.Spell();
        }

        public void ResetShape()
        {

            GameManager.instance.gobackhome();
            
        }

        public void PencilClickEvent(Pencil pencil)
        {
            if (pencil == null)
            {
                return;
            }
            if (GameManager.instance == null)
            {
                return;
            }
            if (GameManager.instance.currentPencil != null)
            {
                GameManager.instance.currentPencil.DisableSelection();
                GameManager.instance.currentPencil = pencil;
            }
            GameManager.instance.SetShapeOrderColor();
            pencil.EnableSelection();
        }

        public void HelpUserBoosterClick(Booster booster)
        {
            if (booster == null || !GameManager.instance.isRunning)
            {
                return;
            }
           
            if (booster.GetValue() == 0)
            {
                AudioSources.instance.PlayLockedSFX();
                ShowRenewHelpBoosterDialog();
                return;
            }

            booster.DecreaseValue();

            GameManager.instance.HelpUser();
        }

        public void RenewHelpBooster(Booster booster)
        {
            if (booster == null)
            {
                return;
            }

            GameManager.instance.Resume();
            renewHelpBoosterDialog.Hide(true);

            //Set yes button of the dialog as interactable false
            renewHelpBoosterDialog.transform.Find("YesButton").GetComponent<Button>().interactable = false;

            //Renew booster value when the Ad is sucessfully appeared
            unityEvent.RemoveAllListeners();
            unityEvent.AddListener(() => booster.ResetValue());
            unityEvent.AddListener(() => renewHelpBoosterDialog.transform.Find("YesButton").GetComponent<Button>().interactable = true);

            AdsManager.instance.ShowAdvertisment(AdPackage.AdEvent.Event.ON_RENEW_HELP_COUNT, unityEvent);
        }

        public void ResetShapeClickEvent()
        {
            GameManager.instance.ResetShape();
            Progress.instance.ResetDropFlags();
        }

        public void ShowResetGameDialog()
        {
            GameObject.Find("ResetGameConfirmDialog").GetComponent<Dialog>().Show(true);
        }

        public void ShowRenewHelpBoosterDialog()
        {
            GameManager.instance.Pause();
            renewHelpBoosterDialog.Show(true);
        }

        public void ResetGame()
        {
            DataManager.ResetGame();
        }

        public void LeaveApp()
        {
            Application.Quit();
        }
    }
}