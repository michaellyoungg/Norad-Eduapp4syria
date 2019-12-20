using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;
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
    public class CompoundShape : MonoBehaviour
    {
        /// <summary>
        /// Scale Factor
        /// </summary>
        [Range(0.01f, 2)]
        public float scaleFactor = 0.3f;

        /// <summary>
        /// The shapes list.
        /// </summary>
        public List<Shape> shapes = new List<Shape>();

        /// <summary>
        /// The text of the compound shape.
        /// </summary>
        [SerializeField]
        [TextArea(5, 5)]
        public string text = "";

        /// <summary>
        /// Whether to enable the shapes priority order or not.
        /// </summary>
        //public bool enablePriorityOrder = true;

        // Use this for initialization
        void Start()
        {
        }


        /// <summary>
        /// Get the index of the current shape.
        /// </summary>
        /// <returns>The current shape index.</returns>
        public int GetCurrentShapeIndex()
        {
            int index = -1;
            bool isCurrentPath;
            for (int i = 0; i < shapes.Count; i++)
            {

                if (shapes[i].completed)
                {
                    continue;
                }

                //current if all of previous shapes of it is completed
                isCurrentPath = true;
                for (int j = 0; j < i; j++)
                {
                    if (!shapes[j].completed)
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
        /// Gets the shape's index by instance ID.
        /// </summary>
        /// <returns>The shape index by instance Id.</returns>
        /// <param name="ID">Instance ID.</param>
        public int GetShapeIndexByInstanceID(int ID)
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                if (ID == shapes[i].GetInstanceID())
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Determine whether the compound shape is completed.
        /// </summary>
        /// <returns><c>true</c> if this instance is completed; otherwise, <c>false</c>.</returns>
        public bool IsCompleted()
        {
            bool completed = true;
            foreach (Shape shape in shapes)
            {
                if (!shape.completed)
                {
                    completed = false;
                    break;
                }
            }
            return completed;
        }


#if UNITY_EDITOR

        public Vector2 characterOffset = new Vector2(4.5f, 6);
        public Vector2 sentenceOffset = Vector2.zero;

        public AudioClip audioClip;
        public Sprite addonPicture;

        public void ApplyScale(){
            transform.localScale = Vector3.one * scaleFactor * CommonUtil.ShapeGameAspectRatio();
        }

        public void ApplyOffset()
        {
            transform.localPosition = new Vector3(sentenceOffset.x, sentenceOffset.y, 0);
        }

        public void Generate()
        {
            //build shapes mapping list for the sentences
            Dictionary<string, GameObject> shapesMapping = new Dictionary<string, GameObject>();
            List<ShapesManager> shapesManagers = GameObject.FindObjectOfType<SingletonManager>().GetShapesManagers(new string[] { "SShapesManager" });

            string shapeName = "";
            foreach (ShapesManager sm in shapesManagers)
            {
                foreach (ShapesManager.Shape shape in sm.shapes)
                {
                    if (shape.prefab == null) continue;
                    shapeName = shape.prefab.name.Split('-')[0];
                    if (!shapesMapping.ContainsKey(shapeName))
                    {
                        shapesMapping.Add(shapeName, shape.prefab);
                    }
                }
            }

            //generate the sentence based on the text

            foreach (Shape shape in shapes)
            {
                if (shape != null)
                    DestroyImmediate(shape.gameObject);
            }

            shapes.Clear();

            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
            {
                children.Add(child);
            }
            foreach (Transform child in children)
            {
                DestroyImmediate(child.gameObject);
            }

            text = text.Trim();

            gameObject.name = text.Replace('\n', ' ') + "-sentence";

            string[] lines = text.Split('\n');

            ApplyScale();
            ApplyOffset();

            Vector2 tempCharacterOffset;
            tempCharacterOffset.x = characterOffset.x * scaleFactor * CommonUtil.ShapeGameAspectRatio();
            tempCharacterOffset.y = characterOffset.y * scaleFactor * CommonUtil.ShapeGameAspectRatio();

            float xPos, yPos, extraOffset;
            for (int i = 0; i < lines.Length; i++)
            {

                if (lines.Length % 2 == 0)
                {//even # of lines
                    extraOffset = tempCharacterOffset.y / 2.0f;
                }
                else
                {//odd # of lines
                    extraOffset = 0;
                }

                //new line
                yPos = transform.position.y + Mathf.FloorToInt(lines.Length / 2.0f) * tempCharacterOffset.y - (i * tempCharacterOffset.y) - extraOffset;

                for (int j = 0; j < lines[i].Length; j++)
                {

                    GameObject shapePrefab = null;
                    bool shapeExists = shapesMapping.TryGetValue(lines[i][j].ToString(), out shapePrefab);
                    if (shapeExists)
                    {

                        if (lines[i].Length % 2 == 0)
                        {//even # of characters
                            extraOffset = tempCharacterOffset.x / 2.0f;
                        }
                        else
                        {//odd # of characters
                            extraOffset = 0;
                        }

                        xPos = transform.position.x - Mathf.FloorToInt(lines[i].Length / 2.0f) * tempCharacterOffset.x + (j * tempCharacterOffset.x) + extraOffset;

                        //create new shape
                        CreateNewShape(shapePrefab, new Vector3(xPos, yPos, 0));
                    }
                }
            }

        }


        private void CreateNewShape(GameObject prefab, Vector3 pos)
        {
            if (prefab == null)
            {
                return;
            }

            GameObject shapeGO = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            shapeGO.transform.SetParent(transform);
            shapeGO.name = prefab.name;
            shapeGO.transform.position = pos;
            shapeGO.transform.localScale = prefab.transform.localScale;
            shapes.Add(shapeGO.GetComponent<Shape>());
        }

#endif
    }
}