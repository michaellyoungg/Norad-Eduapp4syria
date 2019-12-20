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

namespace IndieStudio.EnglishTracingBook.Utility
{
    [DisallowMultipleComponent]
    public class OSCursorManager : MonoBehaviour
    {
        /// <summary>
        /// The status of the OS cursor.
        /// </summary>
        public CursorStatus status = CursorStatus.ENABLED;

        // Update is called once per frame
        void Start()
        {
            #if (!(UNITY_ANDROID || UNITY_IPHONE) || UNITY_EDITOR)
                if (status == CursorStatus.ENABLED)
                {
                    Cursor.visible = true;
                }
                else if (status == CursorStatus.DISABLED)
                {
                    Cursor.visible = false;
                }
            #endif
        }

        public enum CursorStatus
        {
            ENABLED,
            DISABLED
        };
    }
}