using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public class Booster : MonoBehaviour
    {
        /// <summary>
        /// The number text of the booster.
        /// </summary>
        public Text number;

        /// <summary>
        /// The type of the booster.
        /// </summary>
        public Type type;

        /// <summary>
        /// The remaining value/count of the booster.
        /// </summary>
        private int value;

        /// <summary>
        /// References array.
        /// You can attach references/objects to be accessed
        /// </summary>
        public Transform[] references;

        // Use this for initialization
        void Awake()
        {
            //Load booster value
            value = LoadValue();
            if (value == -1)
            {
                //If there is no data , use default value
                ResetValue();
            }

            //Refresh/Set the number text value
            RefreshNumberText();
        }

        /// <summary>
        /// Get the value/count of the booster.
        /// </summary>
        /// <returns>The value/count of the booster.</returns>
        public int GetValue()
        {
            return value;
        }

        /// <summary>
        /// Decrease booster's value by one.
        /// </summary>
        public void DecreaseValue()
        {
            if (value == 0)
            {
                return;
            }

            value--;
            RefreshNumberText();
            SaveValue();
        }

        /// <summary>
        /// Load booster's value/count.
        /// </summary>
        /// <returns>The value/count of the booster.</returns>
        public int LoadValue()
        {
            if (type == Type.HELP_USER)
            {
                return DataManager.GetHelpCountValue();
            }
            return 0;
        }

        /// <summary>
        /// Save the value/count of the booster.
        /// </summary>
        public void SaveValue()
        {
            if (type == Type.HELP_USER)
            {
                DataManager.SaveHelpCountValue(value);
            }
        }

        /// <summary>
        /// Reset the value/count of the booster.
        /// </summary>
        public void ResetValue()
        {
            value = 3;

            if (type == Type.HELP_USER)
            {
                DataManager.SaveHelpCountValue(value);
            }
            RefreshNumberText();
        }

        /// <summary>
        /// Refresh number text value.
        /// </summary>
        private void RefreshNumberText()
        {
            if (number != null)
                number.text = value.ToString();
        }

        public enum Type
        {
            HELP_USER,
        };

    }
}