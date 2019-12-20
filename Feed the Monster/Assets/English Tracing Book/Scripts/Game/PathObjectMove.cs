using IndieStudio.EnglishTracingBook.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public class PathObjectMove : MonoBehaviour
    {
        /// <summary>
        /// The curve reference.
        /// </summary>
        public Curve curve;

        /// <summary>
        /// The image component
        /// </summary>
        public Image image;

        /// <summary>
        /// The transform scale component
        /// </summary>
        public TransformScale transformScale;

        /// <summary>
        /// Whether to move on start or not
        /// </summary>
        public bool moveOnStart;

        /// <summary>
        /// Whether to make the object look at the target or not
        /// </summary>
        public bool allowLook = false;

        /// <summary>
        /// The target point to move.
        /// </summary>
        [HideInInspector]
        public Vector2 target;

        /// <summary>
        /// The speed of the movement.
        /// </summary>
        [Range(0, 10)]
        public float speed = 3f;

        /// <summary>
        /// The look or rotation angle speed.
        /// </summary>
        [Range(0, 10)]
        public float lookSpeed = 5;

        /// <summary>
        /// Whether object reached the target point or not.
        /// </summary>
        [HideInInspector]
        public bool reachedTarget;

        /// <summary>
        /// The minimum reach offset.
        /// </summary>
        [Range(0, 1)]
        public float minReachOffset = 0.1f;

        /// <summary>
        /// The follow offset.
        /// </summary>
        public Vector2 followOffset = Vector2.zero;

        /// <summary>
        /// The on reach new point event.
        /// </summary>
        private UnityEvent onReachNewPointEvent;

        /// <summary>
        /// The index of the target vector point.
        /// </summary>
        private int current;

        /// <summary>
        /// The path object reference.
        /// </summary>
        private Transform pathObject;

        /// <summary>
        /// Whether the movement is loop or not.
        /// </summary>
        public bool loop = true;

        /// <summary>
        /// Whether to reverse the movement or not.
        /// </summary>
        private bool reverse;

        /// <summary>
        /// Whether move process is running or not in the Update.
        /// </summary>
        private bool isRunning;

        // Use this for initialization
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isRunning || reachedTarget || curve == null)
            {
                return;
            }

            Follow();
        }

        //Init and Setting up references
        public void Init()
        {
            if (curve == null)
            {
                curve = GetComponent<Curve>();
            }

            if (pathObject == null)
            {
                pathObject = GetComponent<Transform>();
            }

            if (image == null)
            {
                image = GetComponent<Image>();
            }

            if (transformScale == null)
            {
                transformScale = GetComponent<TransformScale>();
            }

            image.enabled = false;

            minReachOffset = Mathf.Abs(minReachOffset);
            reachedTarget = false;

            if (onReachNewPointEvent == null)
            {
                onReachNewPointEvent = new UnityEvent();
            }

            onReachNewPointEvent.AddListener(() => NextPoint());

            current = 0;

            if (!moveOnStart)
            {
                Stop();
            }
            else
            {
                Move();
            }
        }

        /// <summary>
        /// Follow the target
        /// </summary>
        private void Follow()
        {
            //follow the target's postion
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (allowLook)
            {
                //look at the arget
                Vector3 eulerAngle = transform.eulerAngles;
                Vector2 direction = Vector2.zero;
                direction.x = transform.position.x - target.x;
                direction.y = transform.position.y - target.y;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

                eulerAngle.z = Mathf.LerpAngle(eulerAngle.z, angle, lookSpeed * Time.smoothDeltaTime);
                transform.eulerAngles = eulerAngle;
            }

            //check whether reached the target or not
            if (NearByTarget())
            {
                reachedTarget = true;
                onReachNewPointEvent.Invoke();
            }
        }

        /// <summary>
        /// Reverse the movement.
        /// </summary>
        public void Reverse()
        {
            reverse = !reverse;
            NextPoint();
        }

        /// <summary>
        /// Determines whether this instance is reversed or not.
        /// </summary>
        /// <returns><c>true</c> if this instance is reversed; otherwise, <c>false</c>.</returns>
        public bool IsReversed()
        {
            return reverse;
        }

        /// <summary>
        /// Moves to start point.
        /// </summary>
        public void MoveToStart()
        {
            if (curve != null)
            {
                if (curve.points.Count != 0)
                    transform.position = curve.points[0].position;

                if (curve.points.Count == 1)
                {
                    transformScale.Run();
                }
                else if (curve.points.Count > 1)
                {
                    transformScale.Stop();
                }
            }

            MoveTo(0);
        }

        /// <summary>
        /// Move this instance.
        /// </summary>
        public void Move()
        {
            image.enabled = true;
            isRunning = true;
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop()
        {
            image.enabled = false;
            isRunning = false;
            reachedTarget = false;
            current = 0;
        }

        /// <summary>
        /// Moves to point by index.
        /// </summary>
        /// <param name="index">Index.</param>
        private void MoveTo(int index)
        {
            if (curve == null)
            {
                return;
            }

            if (index > -1 && index < curve.GetBezierPointsCount())
            {
                Reset();
                target = curve.GetBezierPoints()[index];
                Move();
            }
        }

        /// <summary>
        /// Move to the next point.
        /// </summary>
        private void NextPoint()
        {
            if (reverse)
            {
                current--;
            }
            else
            {
                current++;
            }

            /*
            //Destroy path object on reach the end of the path
            if (curve.destoryObjectsOnReachEnd && current == curve.GetBezierPointsCount())
            {
                Destroy(pathObject.gameObject);
                return;
            }*/

            if (loop)
            {
                if (reverse)
                {
                    if (current == -1)
                    {
                        current = curve.GetBezierPointsCount() - 1;
                    }
                }
                else
                {
                    if (current == curve.GetBezierPointsCount())
                    {
                        //set position to the first point
                        //transform.position = curve.GetFirstPoint().position;
                        current = 0;
                    }
                }

            }

            MoveTo(current);
        }

        /// <summary>
        /// Whether object is nearby the target point or not.
        /// </summary>
        /// <returns><c>true</c>, if the target was close, <c>false</c> otherwise.</returns>
        private bool NearByTarget()
        {
            return Vector2.Distance(target, transform.position) <= TotalOffset();
        }

        /// <summary>
        /// The total offset between the object and the target point.
        /// </summary>
        /// <returns>The offset.</returns>
        private float TotalOffset()
        {
            return minReachOffset + followOffset.magnitude;
        }

        /// <summary>
        /// Reset this instance.
        /// </summary>
        private void Reset()
        {
            isRunning = false;
            reachedTarget = false;
        }
    }
}