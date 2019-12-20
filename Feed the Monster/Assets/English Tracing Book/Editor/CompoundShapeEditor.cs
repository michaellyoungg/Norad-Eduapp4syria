using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
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

[CustomEditor (typeof(CompoundShape))]
public class CompoundShapeEditor : Editor
{   
	private string previousText = null;
	private Vector2 previousCharacterOffset = Vector2.zero, previousSentenceOffset = Vector2.zero;
	private float previousSentencesScaleFactor = -1;
	private static bool showShapes = true;
    private List<ShapesManager> shapesManagers = new List<ShapesManager>();
    private string[] positionsInshapeManagerNames;
    private static string[] shapesManagersNames;
    private List<int> SMShapesIndexes = new List<int>();
    private int selectedShapesManager, previousSelectedShapesManager = -1, positionInSelectedShapesManager;
    private string btnName;
    private bool compoundShapeAlreadyExits;
    private GameObject loadedObject;

	public override void OnInspectorGUI ()
	{
        if (Application.isPlaying)
        {
            return;
        }

		CompoundShape compoundShape = (CompoundShape)target;//get the target

		if (PrefabUtility.GetPrefabType (compoundShape.gameObject) == PrefabType.Prefab) {
            EditorGUILayout.HelpBox("You are only allowed to edit the attributes in the Hierarchy layout. Drag and drop the prefab to the Hierarchy under Shape gameobject to edit it", MessageType.Warning);
			return;
        }

        if (previousText == null) previousText = compoundShape.text;
        if (previousCharacterOffset.magnitude == Vector2.zero.magnitude) previousCharacterOffset = compoundShape.characterOffset;
        if (previousSentenceOffset.magnitude == Vector2.zero.magnitude) previousSentenceOffset = compoundShape.sentenceOffset;
        if (previousSentencesScaleFactor == -1) previousSentencesScaleFactor = compoundShape.scaleFactor;

        btnName = "Create Prefab";
        compoundShapeAlreadyExits = false;

        loadedObject = null;
        if (compoundShape != null)
            loadedObject = AssetDatabase.LoadAssetAtPath("Assets/English Tracing Book/Prefabs/Sentences/" + compoundShape.text.Trim().Replace("\n", "") + "-sentence.prefab", typeof(GameObject)) as GameObject;

        if (loadedObject != null)
        if (loadedObject.GetComponent<CompoundShape>().text.Trim() == compoundShape.text.Trim())
        {
            compoundShapeAlreadyExits = true;
        }
        
        EditorGUILayout.Separator();
        #if !(UNITY_5 || UNITY_2017 || UNITY_2018_0 || UNITY_2018_1 || UNITY_2018_2)
            //Unity 2018.3 or higher
            if (compoundShapeAlreadyExits && PrefabUtility.GetCorrespondingObjectFromSource(compoundShape.gameObject) !=null)
            {
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Colors.cyanColor;
                EditorGUILayout.Separator();
                if (GUILayout.Button("Apply", GUILayout.Width(70), GUILayout.Height(30), GUILayout.ExpandWidth(false)))
                {
                    PrefabUtility.ApplyPrefabInstance(compoundShape.gameObject, InteractionMode.AutomatedAction);
                }
                GUI.backgroundColor = Colors.whiteColor;
                EditorGUILayout.EndHorizontal();
            }
        #endif
        EditorGUILayout.Separator();

        EditorGUILayout.HelpBox("Click on the Create button to save the sentence in the Prefabs folder", MessageType.Info);
        EditorGUILayout.HelpBox("You can move the shapes Manually in the sentence or word", MessageType.Info);
        EditorGUILayout.HelpBox("Click on the top Apply button to apply changes on the sentence's prefab", MessageType.None);

        if (shapesManagers.Count == 0)
        {
            shapesManagers = GameManager.FindObjectOfType<SingletonManager>().GetShapesManagers(null);

            shapesManagersNames = new string[shapesManagers.Count];
            for (int i = 0; i < shapesManagers.Count; i++)
            {
                if (shapesManagers[i].name == "SShapesManager")
                {
                    selectedShapesManager = i;
                }
                shapesManagersNames[i] = shapesManagers[i].name;
            }
        }

		SerializedObject attrib = new SerializedObject (target);
		attrib.Update ();

		EditorGUILayout.Separator ();

		EditorGUILayout.PropertyField (attrib.FindProperty ("text"), true);
		EditorGUILayout.Separator ();
		EditorGUILayout.PropertyField (attrib.FindProperty ("characterOffset"), true);
		EditorGUILayout.PropertyField (attrib.FindProperty ("scaleFactor"), true);
		EditorGUILayout.PropertyField (attrib.FindProperty ("sentenceOffset"), true);
        attrib.ApplyModifiedProperties();

        GUI.backgroundColor = Colors.whiteColor;    

        if (shapesManagers.Count != 0)
        {
            if (previousSelectedShapesManager != selectedShapesManager)
            {
                previousSelectedShapesManager = selectedShapesManager;
                SMShapesIndexes.Clear();

                //get not null indexes
                for (int i = 0; i < shapesManagers[selectedShapesManager].shapes.Count; i++)
                {
                    if (shapesManagers[selectedShapesManager].shapes[i].prefab == null)
                        continue;

                    SMShapesIndexes.Add(i);
                }

                positionsInshapeManagerNames = new string[SMShapesIndexes.Count + 2];
                positionsInshapeManagerNames[0] = "At the Start";

                for (int i = 1; i < positionsInshapeManagerNames.Length - 1; i++)
                {
                    positionsInshapeManagerNames[i] = "After " + shapesManagers[selectedShapesManager].shapes[SMShapesIndexes[i - 1]].prefab.name;
                }

                positionsInshapeManagerNames[positionsInshapeManagerNames.Length - 1] = "At the End";
                positionInSelectedShapesManager = positionsInshapeManagerNames.Length - 1;
            }
           
            EditorGUILayout.Separator();

            EditorGUILayout.Separator();
            compoundShape.addonPicture = EditorGUILayout.ObjectField("Addon Picture", compoundShape.addonPicture, typeof(Sprite), true) as Sprite;
            EditorGUILayout.Separator();
            compoundShape.audioClip = EditorGUILayout.ObjectField("Audio Clip", compoundShape.audioClip, typeof(AudioClip), true) as AudioClip;
            selectedShapesManager = EditorGUILayout.Popup("Shapes Manager", selectedShapesManager, shapesManagersNames);
            positionInSelectedShapesManager = EditorGUILayout.Popup("Position", positionInSelectedShapesManager, positionsInshapeManagerNames);

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
        }

		showShapes = EditorGUILayout.Foldout (showShapes, "Shapes");
		if (showShapes) {
			for (int i = 0; i < compoundShape.shapes.Count; i++) {
				compoundShape.shapes [i] = EditorGUILayout.ObjectField ("Element " + i, compoundShape.shapes [i], typeof(Shape), true) as Shape;
			}
		}

        EditorGUILayout.BeginVertical();
  
        if (compoundShapeAlreadyExits){
            EditorGUI.BeginDisabledGroup(true);
            btnName = "Prefab Already Exists";
        }
 
        if (previousText != compoundShape.text || Vector2.Distance(previousCharacterOffset, compoundShape.characterOffset) != 0)
        {
            previousText = compoundShape.text;
            previousCharacterOffset = compoundShape.characterOffset;
            compoundShape.Generate();
        }

        if (Vector2.Distance(previousSentenceOffset, compoundShape.sentenceOffset) != 0)
        {
            previousSentenceOffset = compoundShape.sentenceOffset;
            compoundShape.ApplyOffset();
        }

        if (previousSentencesScaleFactor != compoundShape.scaleFactor)
        {
            previousSentencesScaleFactor = compoundShape.scaleFactor;
            compoundShape.ApplyScale();
        }

        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Colors.whiteColor;

        if (compoundShape.shapes.Count != 0)
        {
            GUI.backgroundColor = Colors.greenColor;

            if (btnName != "Create Prefab")
            {
                GUI.backgroundColor = Colors.redColor;
            }
            if (GUILayout.Button(btnName, GUILayout.Width(150), GUILayout.Height(20)))
            {
                CreateCompoundShape(compoundShape);
                return;
            }
        }

        EditorGUI.EndDisabledGroup();
        GUI.backgroundColor = Colors.greenColor;

        if (GUILayout.Button("RE-Generate", GUILayout.Width(120), GUILayout.Height(20)))
        {
            compoundShape.Generate();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.Separator();

		if (GUI.changed)
        {
            DirtyUtil.MarkSceneDirty ();
		}

	}

    private void CreateCompoundShape(CompoundShape compoundShape)
	{
        if (compoundShape == null)
        {
			return;
		}

        string cShapePrefabLocalPath = EditorUtility.OpenFolderPanel("Create Word OR Sentence", Application.dataPath + "/English Tracing Book/Prefabs/Sentences", "");

        if (string.IsNullOrEmpty(cShapePrefabLocalPath))
        {
            return;
        }

        cShapePrefabLocalPath = "Assets" + cShapePrefabLocalPath.Replace(Application.dataPath, "");

        GameObject savedShapePrefab = CommonUtil.SaveAsPrefab(cShapePrefabLocalPath + "/" + compoundShape.gameObject.name + ".prefab", compoundShape.gameObject, false);

        int index = 0;
        if (positionInSelectedShapesManager == 0)
        {
            index = 0;// At the Start
        }
        else if (positionInSelectedShapesManager == positionsInshapeManagerNames.Length - 1)
        {
            index = shapesManagers[selectedShapesManager].shapes.Count;// At the End
        }
        else
        {
            index = SMShapesIndexes[positionInSelectedShapesManager - 1] + 1;// After this shape
        }

        shapesManagers[selectedShapesManager].shapes.Insert(index, new ShapesManager.Shape() { prefab = savedShapePrefab, clip = compoundShape.audioClip, picture = compoundShape.addonPicture });
        shapesManagers[selectedShapesManager].RemoveEmptyShapes();

        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene("Assets/English Tracing Book/Scenes/Main.unity");
        CommonUtil.ReplacePrefab(GameObject.Find(shapesManagers[selectedShapesManager].name), shapesManagers[selectedShapesManager]);
        EditorSceneManager.OpenScene("Assets/English Tracing Book/Scenes/Game.unity");
        
        CompoundShape[] csc = GameObject.FindObjectsOfType<CompoundShape>();
        foreach (CompoundShape cs in csc)
        {
            DestroyImmediate(cs.gameObject);
        }

        EditorUtility.DisplayDialog("Successful Message", "Scentence/Word is created sucessfully in the SShapesManager gameobject in the Main scene", "ok");
	}
}	