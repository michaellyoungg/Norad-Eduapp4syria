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
    public class TransformScale : MonoBehaviour
    {
        /// <summary>
        /// Whether to run scale on start or not
        /// </summary>
        public bool runOnStart;

        /// <summary>
        /// Whether the scale process is running or not
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// The speed of the scale
        /// </summary>
        [Range(0, 10)]
        public float speed = 4;

        /// <summary>
        /// Whehter to do scale for x , y , z coordinates or not
        /// </summary>
        public bool scaleX, scaleY, scaleZ;

        /// <summary>
        /// The target scale we need to follow
        /// </summary>
        public Vector3 targetScale = Vector3.zero;

        /// <summary>
        /// The initial scale of the object
        /// </summary>
        private Vector3 initalScale;

        /// <summary>
        /// A temp scale vectors
        /// </summary>
        private Vector3 tempScale, tepmTarget;

        /// <summary>
        /// Wehther the scale is loop or not (move to the target scale then to the initial scale then reverse, so on and so forth)
        /// </summary>
        public bool loop;

        void Start()
        {
            initalScale = transform.localScale;

            Stop();

            if (runOnStart)
            {
                Run();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isRunning)
            {
                return;
            }

            Scale();
        }

        public void Run()
        {
            isRunning = true;
        }

        public void Stop()
        {
            isRunning = false;
            tepmTarget = targetScale;
            transform.localScale = initalScale;
        }

        private void Scale()
        {
            tempScale = transform.localScale;

            if (scaleX)
            {
                tempScale.x = Mathf.MoveTowards(tempScale.x, tepmTarget.x, speed * Time.deltaTime);
            }
            else
            {
                tepmTarget.x = tempScale.x;
            }

            if (scaleY)
            {
                tempScale.y = Mathf.MoveTowards(tempScale.y, tepmTarget.y, speed * Time.deltaTime);
            }
            else
            {
                tepmTarget.y = tempScale.y;
            }

            if (scaleZ)
            {
                tempScale.z = Mathf.MoveTowards(tempScale.z, tepmTarget.z, speed * Time.deltaTime);
            }
            else
            {
                tepmTarget.z = tempScale.z;
            }

            transform.localScale = tempScale;

            if (Mathf.Approximately(transform.localScale.magnitude, tepmTarget.magnitude))
            {
                transform.localScale = tepmTarget;

                if (loop)
                {
                    tepmTarget = transform.localScale.magnitude == initalScale.magnitude ? targetScale : initalScale;
                }
                else
                {
                    isRunning = false;
                }
            }
        }
    }
}
