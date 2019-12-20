using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using IndieStudio.EnglishTracingBook.Game;
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

[CustomEditor(typeof(TracingPath))]
public class PathEditor : Editor
{
    private Shape shape;

    public override void OnInspectorGUI()
    {
        TracingPath tracingPath = (TracingPath)target;//get the target

        if (shape == null)
        {
            shape = tracingPath.GetComponentInParent<Shape>();
        }

        EditorGUILayout.Separator();
        #if !(UNITY_5 || UNITY_2017 || UNITY_2018_0 || UNITY_2018_1 || UNITY_2018_2)
            //Unity 2018.3 or higher
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Colors.cyanColor;
            EditorGUILayout.Separator();
            if (GUILayout.Button("Apply", GUILayout.Width(70), GUILayout.Height(30), GUILayout.ExpandWidth(false)))
            {
                PrefabUtility.ApplyPrefabInstance(tracingPath.gameObject, InteractionMode.AutomatedAction);
            }
            GUI.backgroundColor = Colors.whiteColor;
            EditorGUILayout.EndHorizontal();
        #endif
        EditorGUILayout.Separator();
        tracingPath.fillMethod = (TracingPath.FillMethod)EditorGUILayout.EnumPopup("Fill Method", tracingPath.fillMethod);
        tracingPath.completeOffset = EditorGUILayout.Slider("Complete Offset", tracingPath.completeOffset, 0, 1);
        tracingPath.firstNumber = EditorGUILayout.ObjectField("First Number", tracingPath.firstNumber, typeof(Transform)) as Transform;
        if (tracingPath.fillMethod != TracingPath.FillMethod.Point)
            tracingPath.secondNumber = EditorGUILayout.ObjectField("Second Number", tracingPath.secondNumber, typeof(Transform)) as Transform;

        EditorGUILayout.HelpBox("Click on the Create Curve button to craete a curve for the Path.", MessageType.Info);
        EditorGUILayout.HelpBox("Click on the Apply button to save your changes.", MessageType.Info);

        if (tracingPath.fillMethod == TracingPath.FillMethod.Point && tracingPath.secondNumber!=null)
        {
            tracingPath.secondNumber.gameObject.SetActive(false);
        }
        else
        {
            tracingPath.secondNumber.gameObject.SetActive(true);
        }

        string btnLabel = "";
       Transform curveTransform = tracingPath.transform.Find("Curve");

        if (curveTransform != null)
        {
            btnLabel = "Curve is already created";
            EditorGUI.BeginDisabledGroup(true);
        }
        else
        {
            btnLabel = "Create Curve";
        }
        GUI.color = Colors.greenColor;
        if (GUILayout.Button(btnLabel, GUILayout.ExpandWidth(true), GUILayout.Height(25)))
        {
            Curve.CreateNewCurve();
        }
        EditorGUI.EndDisabledGroup();

        GUI.color = Colors.paleGreen;
        if (curveTransform != null && tracingPath.fillMethod != TracingPath.FillMethod.Point)
        if (GUILayout.Button("Flip Direction", GUILayout.ExpandWidth(true), GUILayout.Height(25)))
        {
            Curve curve = curveTransform.GetComponent<Curve>();

            Vector3 temp;
            for (int i = 0 ; i < curve.points.Count/2 ;i++)
            {
                temp = curve.points[i].position;
                curve.points[i].position = curve.points[curve.points.Count - 1 - i].position;
                curve.points[curve.points.Count - 1 - i].position = temp; 
            }

            if (tracingPath.fillMethod == TracingPath.FillMethod.Linear){
                if (tracingPath.fillImage.fillMethod == Image.FillMethod.Horizontal)
                {
                    if (tracingPath.fillImage.fillOrigin == (int)Image.OriginHorizontal.Right)
                        tracingPath.fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
                    else if (tracingPath.fillImage.fillOrigin == (int)Image.OriginHorizontal.Left)
                        tracingPath.fillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
                }
                else if (tracingPath.fillImage.fillMethod == Image.FillMethod.Vertical)
                {
                    if (tracingPath.fillImage.fillOrigin == (int)Image.OriginVertical.Top)
                        tracingPath.fillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
                    else if (tracingPath.fillImage.fillOrigin == (int)Image.OriginVertical.Bottom)
                        tracingPath.fillImage.fillOrigin = (int)Image.OriginVertical.Top;
                }
            }
            else if (tracingPath.fillMethod == TracingPath.FillMethod.Radial)
            {
                tracingPath.fillImage.fillClockwise = !tracingPath.fillImage.fillClockwise;
            }
           EditorUtility.DisplayDialog("Message", "Path's direction is flipped sucessfully", "ok");
        }

        GUI.color = Colors.whiteColor;

        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        EditorGUILayout.BeginHorizontal();

        if (shape == null)
        {
            return;
        }

        int pathIndex = shape.GetPathIndex(tracingPath);

        if (pathIndex == -1)
        {
            return;
        }

        if (pathIndex == 0)
            EditorGUI.BeginDisabledGroup(true);

        if (GUILayout.Button("◄ Previous Path", GUILayout.Width(120), GUILayout.Height(25)))
        {
            Selection.activeObject = shape.tracingPaths[pathIndex - 1];
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Separator();

        if (pathIndex == shape.tracingPaths.Count - 1)
            EditorGUI.BeginDisabledGroup(true);

        if (GUILayout.Button("Next Path ►", GUILayout.Width(120), GUILayout.Height(25)))
        {
            Selection.activeObject = shape.tracingPaths[pathIndex + 1];
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUI.changed)
        {
            DirtyUtil.MarkSceneDirty();
        }
    }
}

