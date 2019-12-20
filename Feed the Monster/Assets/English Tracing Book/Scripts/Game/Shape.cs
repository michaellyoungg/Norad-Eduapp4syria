
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    public class Shape : MonoBehaviour
    {
        /// <summary>
        /// The paths of the shape.
        /// </summary>
        public List<TracingPath> tracingPaths = new List<TracingPath>();

        /// <summary>
        /// Whether the shape is completed or not.
        /// </summary>
        [HideInInspector]
        public bool completed;

        /// <summary>
        /// Whether to enable the priority order or not.
        /// </summary>
        [HideInInspector]
        public bool enablePriorityOrder = true;

        /// <summary>
        /// The animator of the shape
        /// </summary>
        public Animator animator;

        /// <summary>
        /// The content gameobject under the shape
        /// </summary>
        [HideInInspector]
        public Transform content;

        /// <summary>
        /// The id of the shape
        /// </summary>
        [HideInInspector]
        public int ID;

        // Use this for initialization
        void Awake()
        {
            if (content == null)
            {
                content = transform.Find("Content");
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        void Start()
        {
        }

        /// <summary>
        /// Show the numbers of the path .
        /// </summary>
        /// <param name="index">Index.</param>
        public void ShowPathNumbers(int index)
        {
            for (int i = 0; i < tracingPaths.Count; i++)
            {
                if (i != index)
                {
                    tracingPaths[i].SetNumbersStatus(false);
                    if (enablePriorityOrder || tracingPaths[i].completed)
                    {
                        tracingPaths[i].curve.gameObject.SetActive(false);
                    }
                }
                else
                {
                    tracingPaths[i].SetNumbersStatus(true);
                    tracingPaths[i].curve.gameObject.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Get the index of the current path.
        /// </summary>
        /// <returns>The current path index.</returns>
        public int GetCurrentPathIndex()
        {
            int index = -1;
            for (int i = 0; i < tracingPaths.Count; i++)
            {
                if (tracingPaths[i].completed)
                {
                    continue;
                }

                //current if all of previous paths of it is completed
                bool isCurrentPath = true;
                for (int j = 0; j < i; j++)
                {
                    if (!tracingPaths[j].completed)
                    {
                        isCurrentPath = false;
                        break;
                    }
                }

                if (isCurrentPath)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Determine whether the given tracing path instance is the current path or not.
        /// </summary>
        /// <returns><c>true</c> if this instance is current path; otherwise, <c>false</c>.</returns>
        /// <param name="path">Tracing Path.</param>
        public bool IsCurrentPath(TracingPath tracingPath)
        {
            bool isCurrentPath = false;

            if (!enablePriorityOrder)
            {
                return true;
            }

            if (tracingPath == null)
            {
                return isCurrentPath;
            }

            isCurrentPath = true;
            for (int i = 0; i < tracingPaths.Count; i++)
            {
                if (tracingPaths[i].GetInstanceID() == tracingPath.GetInstanceID())
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (!tracingPaths[j].completed)
                        {
                            isCurrentPath = false;
                            break;
                        }
                    }
                    break;
                }
            }

            return isCurrentPath;
        }

        /// <summary>
        /// Get index of the tracing path
        /// </summary>
        /// <param name="tracingPath">Tracing Path Reference</param>
        /// <returns>Index of the Tracing Path in the List</returns>
        public int GetPathIndex(TracingPath tracingPath)
        {
            if (tracingPath == null)
                return -1;

            for (int i = 0; i < tracingPaths.Count; i++)
            {
                if (tracingPaths[i] == null)
                    continue;

                if (tracingPaths[i].GetInstanceID() == tracingPath.GetInstanceID())
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Get the title of the shape.
        /// </summary>
        /// <returns>The title.</returns>
        public string GetTitle()
        {
            if (GameManager.instance.compoundShape == null)
            {
                return name.Split('-')[0];
            }
            return GameManager.instance.compoundShape.name.Split('-')[0];
        }

        /// <summary>
        /// Get the shape instance in the shapes manager
        /// </summary>
        /// <returns></returns>
        public ShapesManager.Shape GetShapeFromShapesManager()
        {
            if (ShapesManager.GetCurrentShapesManager() == null)
                return null;
            if (ID < 0 || ID > ShapesManager.GetCurrentShapesManager().shapes.Count - 1)
                return null;

            return ShapesManager.GetCurrentShapesManager().shapes[ID];
        }
    }
}