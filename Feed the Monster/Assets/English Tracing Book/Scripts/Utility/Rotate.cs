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
    public class Rotate : MonoBehaviour
    {

        /// <summary>
        /// Whether rotation is enabled or not.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// The speed of rotation.
        /// </summary>
        public float speed = 10;

        /// <summary>
        /// The rotate direction.
        /// </summary>
        private Vector3 direction = new Vector3(0, 0, 1);

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Enabled)
            {
                transform.Rotate(direction * speed * Time.smoothDeltaTime);
            }
        }
    }
}