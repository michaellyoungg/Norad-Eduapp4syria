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
    [ExecuteInEditMode]
    public class Line : MonoBehaviour
    {
        /// <summary>
        /// The line renderer reference.
        /// </summary>
        public LineRenderer lineRenderer;

        /// <summary>
        /// The material of the line.
        /// </summary>
        public Material material;

        /// <summary>
        /// The color gradient of the line.
        /// </summary>
        public Gradient color;

        /// <summary>
        /// The width of the line.
        /// </summary>
        [Range(0, 10)]
        public float width = 1f;

        /// <summary>
        /// The minimum offset between points.
        /// </summary>
        [Range(0, 10)]
        public float offsetBetweenPoints = 6f;

        /// <summary>
        /// The point Z position.
        /// </summary>
        [Range(-20, 20)]
        public float pointZPosition;

        /// <summary>
        /// The sorting order of line
        /// </summary>
        public int sortingOrder;

        /// <summary>
        /// The points of the line.
        /// </summary>
        [HideInInspector]
        public List<Vector3> points = new List<Vector3>();

        //Drawing points list of the bezier path
        private List<Vector3> bezierDrawingPoints;

        /// <summary>
        /// Bezier instance
        /// </summary>
        private static Bezier bezier;

        void Awake()
        {
            Init();
        }

        void Update()
        {
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public void Init()
        {
            if (bezier == null)
            {
                bezier = new Bezier();
            }

            if (material == null)
            {
                material = new Material(Shader.Find("Sprites/Default"));
            }

            points = new List<Vector3>();
            lineRenderer = GetComponent<LineRenderer>();
            GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            if (lineRenderer != null)
            {
                lineRenderer.material = material;
                lineRenderer.widthMultiplier = width;
            }
        }

        /// <summary>
        /// Add the point to the line.
        /// </summary>
        /// <param name="point">Point.</param>
        public void AddPoint(Vector3 point)
        {
            if (lineRenderer != null)
            {
                point.z = pointZPosition;
                if (points.Count > 1)
                {
                    if (Vector3.Distance(point, points[points.Count - 1]) < offsetBetweenPoints)
                    {
                        return;//skip the point
                    }
                }

                points.Add(point);
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPosition(points.Count - 1, point);
            }
        }

        /// <summary>
        /// Set the material of the line.
        /// </summary>
        /// <param name="material">Material.</param>
        public void SetMaterial(Material material)
        {
            this.material = material;
            if (lineRenderer != null)
            {
                lineRenderer.material = this.material;
            }
        }

        /// <summary>
        /// Sets the width of the line.
        /// </summary>
        /// <param name="width">Line width.</param>
        public void SetWidth(float width)
        {
            if (lineRenderer != null)
            {
                lineRenderer.widthMultiplier = width;
            }
            this.width = width;
        }

        /// <summary>
        /// Set the color of the line.
        /// </summary>
        /// <param name="value">Value.</param>
        public void SetColor(Gradient color)
        {

            if (lineRenderer != null)
            {
                lineRenderer.colorGradient = color;
            }

            this.color = color;
        }

        /// <summary>
        /// Set the sorting order of the line.
        /// </summary>
        /// <param name="sortingOrder">Sorting order.</param>
        public void SetSortingOrder(int sortingOrder)
        {

            if (lineRenderer != null)
            {
                lineRenderer.sortingOrder = sortingOrder;
            }
            this.sortingOrder = sortingOrder;
        }

        /// <summary>
        /// Bezier interpolate to smooth the line's curve.
        /// </summary>
        public void BezierInterpolate(float somothness)
        {
            if (points.Count < 2)
                return;

            bezier.Interpolate(points, somothness);
            bezierDrawingPoints = bezier.GetDrawingPoints2();
            lineRenderer.positionCount = bezierDrawingPoints.Count;
            lineRenderer.SetPositions(bezierDrawingPoints.ToArray());
        }

        /// <summary>
        /// Get the points count of the line.
        /// </summary>
        /// <returns>The points count.</returns>
        public int GetPointsCount()
        {
            return this.points.Count;
        }

        public void Reset()
        {
            lineRenderer.positionCount = 0;
            points.Clear();
        }
    }
}