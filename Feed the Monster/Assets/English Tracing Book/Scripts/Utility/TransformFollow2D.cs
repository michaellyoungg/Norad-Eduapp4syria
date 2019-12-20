using UnityEngine;
using System.Collections;
using System;
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

namespace IndieStudio.EnglishTracingBook.Utility
{
    [DisallowMultipleComponent]
    public class TransformFollow2D : MonoBehaviour
    {
        /// <summary>
        /// Whether follow is running or not.
        /// </summary>
        public bool isRunning;

        /// <summary>
        /// The target transform.
        /// </summary>
        public Transform target;

        /// <summary>
        /// Target as Vector3
        /// </summary>
        public Vector3 targetVector;

        /// <summary>
        /// The follow speed.
        /// </summary>
        public float speed = 1;

        /// <summary>
        /// The follow method.
        /// </summary>
        public FollowMethod followMethod = FollowMethod.LERP;

        /// <summary>
        /// The mode of the target / Vector3 or Transform
        /// </summary>
        public TargetMode targetMode = TargetMode.TRANSFORM;

        /// <summary>
        /// The follow offset.
        /// </summary>
        public Vector2 followOffset = Vector2.zero;

        /// <summary>
        /// Whether to follow x , follow y coordinates.
        /// </summary>
        public bool followX = true, followY = true;

        /// <summary>
        /// Whether to apply follow sign.
        /// </summary>
        public bool applyFollowSign;

        /// <summary>
        /// Whether the follow is continuous or not.
        /// </summary>
        public bool continuousFollow;

        /// <summary>
        /// Whether reached target or not.
        /// </summary>
        public bool reachedTarget;

        /// <summary>
        /// A temp vector position.
        /// </summary>
        private Vector2 tempPosition;

        /// <summary>
        /// A temp vector.
        /// </summary>
        private Vector3 tempVector;

        /// <summary>
        /// The minimum reach offset.
        /// </summary>
        private float minReachOffset = 0.01f;

        /// <summary>
        /// On reach target unity event.
        /// </summary>
        public UnityEvent onReachTargeEvent;

        void Start()
        {
            minReachOffset = Math.Abs(minReachOffset);
            reachedTarget = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isRunning)
            {
                return;
            }

            if (targetMode == TargetMode.TRANSFORM && target == null)
            {
                return;
            }

            if (reachedTarget && !continuousFollow)
            {
                return;
            }

            //initial postion
            tempVector = transform.position;

            if (followX)
            {
                tempVector.x = GetValue(transform.position.x, targetMode == TargetMode.TRANSFORM ? target.position.x : targetVector.x + (Mathf.Sign(transform.localScale.x) * followOffset.x));
            }
            else
            {
                tempVector.x = targetMode == TargetMode.TRANSFORM ? target.position.x : targetVector.x;
            }

            if (followY)
            {
                tempVector.y = GetValue(transform.position.y, targetMode == TargetMode.TRANSFORM ? target.position.y : targetVector.y + followOffset.y);
            }
            else
            {
                tempVector.y = targetMode == TargetMode.TRANSFORM ? target.position.y : targetVector.y;
            }

            //apply new postion
            transform.position = tempVector;

            if (applyFollowSign)
            {
                //initial scale
                tempVector = transform.localScale;

                if (transform.position.x < (targetMode == TargetMode.TRANSFORM ? target.position.x : targetVector.x) + (Mathf.Sign(transform.localScale.x) * followOffset.x))
                {
                    tempVector.x = Mathf.Abs(tempVector.x);
                }
                else
                {
                    tempVector.x = -Mathf.Abs(tempVector.x);
                }
                transform.localScale = tempVector;
            }

            if (NearByTarget())
            {
                reachedTarget = true;
                if (onReachTargeEvent != null)
                    onReachTargeEvent.Invoke();
            }
        }

        /// <summary>
        /// Whether the object is nearby the target or not.
        /// </summary>
        /// <returns><c>true</c>, if the target is nearby, <c>false</c> otherwise.</returns>
        private bool NearByTarget()
        {
            tempPosition = transform.position;

            if (followX && !followY)
            {
                tempPosition.y = targetMode == TargetMode.TRANSFORM ? target.position.y : targetVector.y;
            }
            else if (followY && !followX)
            {
                tempPosition.x = targetMode == TargetMode.TRANSFORM ? target.position.x : targetVector.x;
            }

            return Vector2.Distance(targetMode == TargetMode.TRANSFORM ? target.position : targetVector, tempPosition) <= TotalOffset();
        }

        /// <summary>
        /// The total reach offset.
        /// </summary>
        /// <returns>The offset.</returns>
        private float TotalOffset()
        {
            return minReachOffset + followOffset.magnitude;
        }

        /// <summary>
        /// Gets the follow value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="currentValue">Current value.</param>
        /// <param name="targetValue">Next Target value.</param>
        private float GetValue(float currentValue, float targetValue)
        {
            float returnValue = 0;

            if (followMethod == FollowMethod.LERP)
            {
                returnValue = Mathf.Lerp(currentValue, targetValue, speed * Time.smoothDeltaTime);
            }
            else if (followMethod == FollowMethod.MOVE_TOWARDS)
            {
                returnValue = Mathf.MoveTowards(currentValue, targetValue, speed * Time.smoothDeltaTime);
            }
            else if (followMethod == FollowMethod.SMOOTH_STEP)
            {
                returnValue = Mathf.SmoothStep(currentValue, targetValue, speed * Time.smoothDeltaTime);
            }
            return returnValue;
        }


        /// <summary>
        /// Move this instance.
        /// </summary>
        public void Move()
        {
            isRunning = true;
        }

        /// <summary>
        /// Reset this instance.
        /// </summary>
        public void Reset()
        {
            target = null;
            isRunning = false;
            reachedTarget = false;
        }

        public enum FollowMethod
        {
            LERP,
            SMOOTH_STEP,
            MOVE_TOWARDS
        };

        public enum TargetMode
        {
            TRANSFORM,
            VECTOR3
        }
    }
}