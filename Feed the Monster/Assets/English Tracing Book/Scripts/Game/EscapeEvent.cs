using UnityEngine;
using System.Collections;
using UnityEngine.Events;

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
    /// <summary>
    /// Escape or Back event
    /// </summary>

    [DisallowMultipleComponent]
    public class EscapeEvent : MonoBehaviour
    {
        /// <summary>
        /// On escape/back event
        /// </summary>
        public UnityEvent escapeEvent;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnEscapeClick();
            }
        }

        /// <summary>
        /// On Escape click event.
        /// </summary>
        public void OnEscapeClick()
        {
            bool visibleDialogFound = HideVisibleDialogs();

            if (visibleDialogFound)
            {
                return;
            }
            escapeEvent.Invoke();
        }

        /// <summary>
        /// Hide the visible dialogs.
        /// </summary>
        /// <returns><c>true</c>, if visible a dialog was visible, <c>false</c> otherwise.</returns>
        private bool HideVisibleDialogs()
        {
            bool visibleDialogFound = false;

            Dialog[] dialogs = GameObject.FindObjectsOfType<Dialog>();
            if (dialogs != null)
            {
                foreach (Dialog d in dialogs)
                {
                    if (d.visible)
                    {
                        if (d.name == "ResetShapeConfirmDialog" || d.name == "RenewHelpBoosterDialog")
                        {
                            //Do not forget to resume game manager, on escape event for this dialog
                            GameManager.instance.Resume();
                        }

                        d.Hide(true);
                        visibleDialogFound = true;
                    }
                }
            }
            return visibleDialogFound;
        }
    }
}