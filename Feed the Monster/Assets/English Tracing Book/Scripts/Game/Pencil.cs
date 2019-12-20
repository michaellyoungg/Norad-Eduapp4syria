using UnityEngine;
using UnityEngine.UI;
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
    public class Pencil : MonoBehaviour
    {
        /// <summary>
        /// The color of the pencil.
        /// </summary>
        public Gradient color;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => UIEvents.instance.PencilClickEvent(this));
        }

        /// <summary>
        /// Enable pencil selection.
        /// </summary>
        public void EnableSelection()
        {
            GetComponent<Animator>().SetBool("RunScale", true);
        }

        /// <summary>
        /// Disable pencil selection.
        /// </summary>
        public void DisableSelection()
        {
            GetComponent<Animator>().SetBool("RunScale", false);
        }
    }
}