using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using IndieStudio.EnglishTracingBook.Utility;
using IndieStudio.EnglishTracingBook.Game;

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

[CustomEditor(typeof(Curve))]
public class CurveEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		SerializedObject attrib = new SerializedObject (target);
        Curve curve = (Curve)target;

		attrib.Update ();

        EditorGUILayout.Separator();
        #if !(UNITY_5 || UNITY_2017 || UNITY_2018_0 || UNITY_2018_1 || UNITY_2018_2)
            //Unity 2018.3 or higher
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Colors.cyanColor;
            EditorGUILayout.Separator();
            if (GUILayout.Button("Apply", GUILayout.Width(70), GUILayout.Height(30), GUILayout.ExpandWidth(false)))
            {
                PrefabUtility.ApplyPrefabInstance(curve.gameObject, InteractionMode.AutomatedAction);
            }
            GUI.backgroundColor = Colors.whiteColor;
            EditorGUILayout.EndHorizontal();
        #endif
		EditorGUILayout.Separator ();
		EditorGUILayout.HelpBox ("Click on 'Add New Point' button or (Shift/CMD + C) to add new Point", MessageType.Info);
		EditorGUILayout.HelpBox ("Click on 'Remove Last Point' button to remove the lastest point in the list", MessageType.Info);
		EditorGUILayout.HelpBox ("Move the Points to set up the curve", MessageType.Info);
        EditorGUILayout.HelpBox("You can change the curve smoothness", MessageType.None);
        EditorGUILayout.HelpBox("You can change the radius of the collider of the point", MessageType.None);
        EditorGUILayout.HelpBox("Collider on each Point used to trigger the tracing event or to know when the user reaches that point"+
            ". Make sure to set the correct size of the collider", MessageType.Info);

		EditorGUILayout.Separator ();

        curve.curveColor = EditorGUILayout.ColorField("Curve Color", curve.curveColor);
		EditorGUILayout.Separator ();
        curve.smoothness = EditorGUILayout.Slider("Curve Smoothness", curve.smoothness,0,0.5f);
        EditorGUILayout.Separator();
        curve.pointColliderRadius = EditorGUILayout.Slider("Point Collider Radius", curve.pointColliderRadius, 0, 10f);
        EditorGUILayout.Separator();

		GUILayout.BeginHorizontal ();
		GUI.backgroundColor = Colors.greenColor;         

		if (GUILayout.Button ("Add New Point", GUILayout.Width (110), GUILayout.Height (20))) {
			curve.CreateNewPoint ();
		}

		GUI.backgroundColor = Colors.redColor;         
		if (GUILayout.Button ("Remove Last Point", GUILayout.Width (150), GUILayout.Height (20))) {
			if (curve.points.Count != 0) {
				if(curve.points[curve.points.Count - 1]!=null)
					DestroyImmediate (curve.points[curve.points.Count - 1].gameObject);
				curve.points.RemoveAt (curve.points.Count - 1);
			}
		}

		GUI.backgroundColor = Colors.whiteColor;
		GUILayout.EndHorizontal ();

		EditorGUILayout.Separator ();

		curve.showContents = EditorGUILayout.Foldout (curve.showContents, "Points");
		if (curve.showContents) {
		for (int i = 0; i <  curve.points.Count; i++) {
				EditorGUILayout.Separator ();
				curve.points [i] = EditorGUILayout.ObjectField ("Point ["+i+"]",  curve.points [i], typeof(Transform), true) as Transform;
				EditorGUILayout.Separator ();
				GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (2));
			}
		}

		attrib.ApplyModifiedProperties ();

		if (GUI.changed)
        {
            DirtyUtil.MarkSceneDirty ();
		}
	}
}