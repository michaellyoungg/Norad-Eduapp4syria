using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public class ShapesManager : MonoBehaviour
    {
        /// <summary>
        /// The shapes list.
        /// </summary>
        public List<Shape> shapes = new List<Shape>();

        /// <summary>
        /// The collected stars in the shapes.
        /// </summary>
        private int collectedStars;

        /// <summary>
        /// The shape label (example Letter or Number).
        /// </summary>
        public string shapeLabel = "Shape";

        /// <summary>
        /// The shape prefix used for DataManager only (example Lowercase or Uppercase or Number).
        /// </summary>
        public string shapePrefix = "Shape";

        /// <summary>
        /// The name of the scene.
        /// </summary>
        public string sceneName = "Album";

        /// <summary>
        /// The title of the album scene.
        /// </summary>
        public Sprite albumTitle;

        /// <summary>
        /// The shape scale factor in the preview/album scene.
        /// </summary>
        public float albumShapeScaleFactor = 0.7f;

        /// <summary>
        /// Line width factor for shapes
        /// </summary>
        public float lineWidthFactor = 1f;

        /// <summary>
        /// The last selected group.
        /// </summary>
        [HideInInspector]
        public int lastSelectedGroup;

        /// <summary>
        /// The tracing mode [ Line or Fill ]
        /// </summary>
        public TracingMode tracingMode;

        /// <summary>
        /// Wehther to enable the test mode or not
        /// All shapes will be unlocked for testing
        /// Set this flag to false on production
        /// </summary>
        public bool testMode = false;

        /// <summary>
        /// Whether to enable shape boundary/bounds limit or not.
        /// (If enabled, then leaveing shape's boundary/bounds will be wrong)
        /// Shape's Collider gameobject used to define the boundary
        /// </summary>
        public bool enableTracingLimit = true;

        /// <summary>
        /// The name of the shapes manager.
        /// </summary>
        public static string shapesManagerReference = "MShapesManager";

        /// <summary>
        /// The list of the shapes managers in the scene.
        /// </summary>
        public static Dictionary<string, ShapesManager> shapesManagers = new Dictionary<string, ShapesManager>();

        void Awake()
        {
            if (shapesManagers.ContainsKey(gameObject.name))
            {
                Destroy(gameObject);
            }
            else
            {
                //init values
                shapesManagers.Add(gameObject.name, this);

                RemoveEmptyShapes();

                for (int i = 0; i < shapes.Count; i++)
                {
                    shapes[i].ID = i;
                    shapes[i].isLocked = DataManager.IsShapeLocked(shapes[i].ID, this);
                    shapes[i].starsNumber = DataManager.GetShapeStars(shapes[i].ID, this);

                    if (shapes[i].ID == 0)
                    {
                        shapes[i].isLocked = false;
                    }

                    if (shapes[i].prefab != null)
                    {
                        if (shapes[i].prefab.GetComponent<IndieStudio.EnglishTracingBook.Game.Shape>() != null)
                            shapes[i].prefab.GetComponent<IndieStudio.EnglishTracingBook.Game.Shape>().ID = shapes[i].ID;
                    }
                }

                collectedStars = CalculateCollectedStars();
               // DontDestroyOnLoad(gameObject);
                lastSelectedGroup = 0;
            }
        }

        void Start()
        {
            SetUpTracingMode();
        }

        private void SetUpTracingMode()
        {
            //Load value from PlayerPrefs
            if (DataManager.GetTracingModeValue() == 0)
            {
                tracingMode = TracingMode.FILL;
            }
            else
            {
                tracingMode = TracingMode.LINE;
            }
        }

        /// <summary>
        /// Calculate the collected stars in all shapes.
        /// </summary>
        /// <returns>The collected stars.</returns>
        private int CalculateCollectedStars()
        {
            int cs = 0;
            foreach (Shape shape in shapes)
            {
                if (shape.starsNumber == ShapesManager.Shape.StarsNumber.ONE)
                {
                    cs += 1;
                }
                else if (shape.starsNumber == ShapesManager.Shape.StarsNumber.TWO)
                {
                    cs += 2;
                }
                else if (shape.starsNumber == ShapesManager.Shape.StarsNumber.THREE)
                {
                    cs += 3;
                }
            }
            return cs;
        }

        /// <summary>
        /// Get the stars rate of the shapes.
        /// </summary>
        /// <returns>The stars rate from 3.</returns>
        public int GetStarsRate()
        {
            return Mathf.FloorToInt(collectedStars / (shapes.Count * 3.0f) * 3.0f);
        }

        /// <summary>
        /// Get the collected stars.
        /// </summary>
        /// <returns>The collected stars.</returns>
        public int GetCollectedStars()
        {
            return collectedStars;
        }

        /// <summary>
        /// Set the collected stars.
        /// </summary>
        /// <param name="collectedStars">Collected stars.</param>
        public void SetCollectedStars(int collectedStars)
        {
            this.collectedStars = collectedStars;
        }

        /// <summary>
        /// Get the current shape.
        /// </summary>
        /// <returns>The current shape.</returns>
        public Shape GetCurrentShape()
        {
            return shapes[Shape.selectedShapeID];
        }

        /// <summary>
        /// Get the current/ selected shapes manager by user
        /// </summary>
        /// <returns></returns>
        public static ShapesManager GetCurrentShapesManager()
        {
            return GameManager.instance.mysm;
        }

        [System.Serializable]
        public class Shape
        {
            /// <summary>
            /// Whether to show the contents/shapes (used for Editor).
            /// </summary>
            public bool showContents = true;

            /// <summary>
            /// The prefab of the shape used in Album/Game scenes.
            /// </summary>
            public GameObject prefab;

            /// <summary>
            /// The picture of the shape.
            /// </summary>
            public Sprite picture;

            /// <summary>
            /// The audio clip of the shape , used for spelling.
            /// </summary>
            public AudioClip clip;

            /// <summary>
            /// The stars time period.
            /// 0 - 14 seconds : 3 stars , 15 - 29 : 2 stars , otherwisee 1 star
            /// </summary>
            public int starsTimePeriod = 15;

            /// <summary>
            /// The number of the collected stars.
            /// </summary>
            [HideInInspector]
            public StarsNumber starsNumber = StarsNumber.ZERO;

            /// <summary>
            /// The ID of the shape.
            /// </summary>
            [HideInInspector]
            public int ID = -1;

            /// <summary>
            /// Whether the shape is locked or not.
            /// </summary>
            [HideInInspector]
            public bool isLocked = true;

            /// <summary>
            /// The ID selected/current shape.
            /// </summary>
            public static int selectedShapeID;

            public enum StarsNumber
            {
                ZERO,
                ONE,
                TWO,
                THREE
            }

            public void Reset()
            {
                if (ID == 0)
                {
                    isLocked = false;
                }
                else
                {
                    isLocked = true;
                }

                starsNumber = StarsNumber.ZERO;
            }
        }

        public void RemoveEmptyShapes()
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (shapes[i].prefab == null)
                {
                    shapes.RemoveAt(i);
                }
            }
        }

        public enum TracingMode
        {
            FILL,
            LINE
        }
    }
}