using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
    public class TracingPath : MonoBehaviour
    {
        /// <summary>
        /// Whether the path is completed or not.
        /// </summary>
        [HideInInspector]
        public bool completed;

        /// <summary>
        /// The fill method (Radial or Linear or Point).
        /// </summary>
        public FillMethod fillMethod;

        /// <summary>
        /// The complete offset (The fill amount offset).
        /// </summary>
        public float completeOffset = 0.85f;

        /// <summary>
        /// The first number reference.
        /// </summary>
        public Transform firstNumber;

        /// <summary>
        /// The second number reference.
        /// </summary>
        public Transform secondNumber;

        /// <summary>
        /// The shape reference.
        /// </summary>
        [HideInInspector]
        public Shape shape;

        /// <summary>
        /// The curve of the tracing path
        /// </summary>
        [HideInInspector]
        public Curve curve;

        /// <summary>
        /// The fill image
        /// </summary>
        [HideInInspector]
        public Image fillImage;

        /// <summary>
        /// The count of the traced points
        /// </summary>
        [HideInInspector]
        public int tracedPoints;

        /// <summary>
        /// The line of the tracing path
        /// </summary>
        [HideInInspector]
        public Line line;

        /// <summary>
        /// From : First Number Value, To : Second Number Value
        /// </summary>
        [HideInInspector]
        public int from, to;

        void Awake()
        {
            string[] slices = gameObject.name.Split('-');
            from = int.Parse(slices[1]);
            to = int.Parse(slices[2]);

            shape = GetComponentInParent<Shape>();
            curve = GetComponentInChildren<Curve>();
            fillImage = CommonUtil.FindChildByTag(transform, "Fill").GetComponent<Image>();
            fillImage.fillAmount = 0;
            tracedPoints = 0;

            //create new line
            CreateNewLine();
        }

        void Start()
        {
           
                if (GameManager.instance.animateNumbersOnStart && GameManager.instance.compoundShape == null)
                {
                    //Animate the numbers of the Path
                    firstNumber.transform.position = secondNumber.transform.position = Vector3.zero;
                    TransformFollow2D firstNTF = firstNumber.gameObject.AddComponent<TransformFollow2D>();
                    TransformFollow2D secondtNTF = secondNumber.gameObject.AddComponent<TransformFollow2D>();

                    firstNTF.target = curve.GetFirstPoint();
                    secondtNTF.target = curve.GetLastPoint();

                    firstNTF.targetMode = secondtNTF.targetMode = TransformFollow2D.TargetMode.TRANSFORM;
                    firstNTF.speed = secondtNTF.speed = 6;
                    firstNTF.transform.position = secondtNTF.transform.position = Vector3.zero;
                    firstNTF.isRunning = secondtNTF.isRunning = true;
                }
            

            //Change numbers parent and sibling to be visible on the top
            RectTransform firstNumberRectTransfrom = firstNumber.GetComponent<RectTransform>();
            RectTransform secondNumberRectTransfrom = secondNumber.GetComponent<RectTransform>();

            firstNumberRectTransfrom.GetComponent<RectTransform>().SetParent(transform.parent);
            secondNumberRectTransfrom.GetComponent<RectTransform>().SetParent(transform.parent);

            firstNumberRectTransfrom.SetAsLastSibling();
            secondNumberRectTransfrom.SetAsLastSibling();
        }

        /// <summary>
        /// Create new line
        /// </summary>
        private void CreateNewLine()
        {
           

            //disable fill image
            fillImage.enabled = false;

            GameObject linePrefab = null;
           
                linePrefab = GameManager.instance.linePrefab;
         

            if (linePrefab != null)
            {
                GameObject lineGO = Instantiate(linePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                lineGO.transform.SetParent(transform);
                Vector3 temp = lineGO.transform.localPosition;
                temp.z = 500;
                
                lineGO.transform.localPosition = temp;
                lineGO.transform.localScale = Vector3.one;
                line = lineGO.GetComponent<Line>();
                line.SetSortingOrder(12);
                SetUpLineWidth();
            }
        }

        public void SetUpLineWidth()
        {
            if (line == null)
                return;

            CompoundShape cs = GetComponentInParent<CompoundShape>();


            if (cs == null)
            {
                //shape line width
                line.SetWidth(shape.content.localScale.magnitude * 1.03f * ShapesManager.GetCurrentShapesManager().lineWidthFactor);
            }
            else
            {
                //compound shape line width
                if (cs.shapes.Count != 0)
                    line.SetWidth(cs.shapes[0].transform.localScale.magnitude * 0.28f * ShapesManager.GetCurrentShapesManager().lineWidthFactor);
            }

        }

        /// <summary>
        /// Auto fill.
        /// </summary>
        public void AutoFill()
        {
            if (Mathf.Approximately(fillImage.fillAmount, 1))
            {
                return;
            }

            StartCoroutine("AutoFillCoroutine");
        }


        /// <summary>
        /// Auto fill coroutine.
        /// </summary>
        /// <returns>The fill coroutine.</returns>
        private IEnumerator AutoFillCoroutine()
        {
            while (fillImage.fillAmount < 1)
            {
                fillImage.fillAmount += 0.02f;
                yield return new WaitForSeconds(0.001f);
            }

        }

        /// <summary>
        /// Set the status of the numbers.
        /// </summary>
        /// <param name="status">the status value.</param>
        public void SetNumbersStatus(bool status)
        {
            Transform[] numbers = new Transform[] { firstNumber, secondNumber };

            Color tempColor = Color.white;
            foreach (Transform number in numbers)
            {
                if (number == null)
                    continue;

                if (status == true)
                {
                    number.GetComponent<Animator>().SetBool("Select", true);
                    tempColor.a = 1;
                }
                else
                {
                    if (shape.enablePriorityOrder)
                    {
                        number.GetComponent<Animator>().SetBool("Select", false);
                        tempColor.a = 0.3f;
                    }
                }

                number.GetComponent<Image>().color = tempColor;
            }
        }

        /// <summary>
        /// Set the visibility of the  numbers.
        /// </summary>
        /// <param name="visible">visibility value.</param>
        public void SetNumbersVisibility(bool visible)
        {
            if (firstNumber != null)
            {
                firstNumber.gameObject.SetActive(visible);
            }
            if (secondNumber != null)
            {
                if (fillMethod == FillMethod.Point)
                {
                    secondNumber.gameObject.SetActive(false);
                }
                else
                {
                    secondNumber.gameObject.SetActive(visible);
                }
            }
        }

        /// <summary>
        /// Disable the current point
        /// </summary>
        public void DisableCurrentPoint()
        {
            curve.SetPointActiveValue(false, tracedPoints);
        }

        /// <summary>
        /// Enable the current point
        /// </summary>
        public void EnableCurrentPoint()
        {
            curve.SetPointActiveValue(true, tracedPoints);
        }

        /// <summary>
        /// Reset the path.
        /// </summary>
        public void Reset()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (line != null)
            {
                line.Reset();
            }

            tracedPoints = 0;
            curve.DisablePoints();
            EnableCurrentPoint();

            SetNumbersVisibility(true);
            completed = false;

            if (!shape.enablePriorityOrder)
            {
                SetNumbersStatus(true);
            }
            StartCoroutine("ReleaseFillCoroutine");
        }


        /// <summary>
        /// Release Fill coroutine.
        /// </summary>
        /// <returns>The coroutine.</returns>
        private IEnumerator ReleaseFillCoroutine()
        {
            while (fillImage.fillAmount > 0)
            {
                fillImage.fillAmount -= 0.02f;
                yield return new WaitForSeconds(0.005f);
            }
        }

        /// <summary>
        /// On complete the tracing path
        /// </summary>
        public void OnComplete()
        {
            SetNumbersVisibility(false);

            //Auto line complete
            if (line != null)
            {
                line.Reset();
                foreach (Transform point in curve.points)
                {
                    line.AddPoint(line.transform.InverseTransformPoint(point.position));
                    if (fillMethod == FillMethod.Point && curve.points.Count == 1)
                    {
                        line.AddPoint(line.transform.InverseTransformPoint(point.position));
                    }
                }
                line.BezierInterpolate(curve.smoothness);
            }
        }

        public enum FillMethod
        {
            Radial,
            Linear,
            Point
        }
    }
}