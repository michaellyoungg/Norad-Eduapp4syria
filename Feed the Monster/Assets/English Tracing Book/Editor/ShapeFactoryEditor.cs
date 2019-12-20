using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using UnityEditor.SceneManagement;
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

public class ShapeFactoryEditor : EditorWindow
{
    private Vector2 scrollPos;
    private Vector2 scale;
    private Vector2 scrollView = new Vector2(550, 430);
    private Vector2 pathSize = new Vector2(60, 60);
    private static ShapeFactoryEditor window;
    private static bool showInstructions = true;
    private static bool addToShapesManager = true;
    private int selectedShapesManager,previousSelectedShapesManager = -1, positionInSelectedShapesManager;
    private static float shapeScale = 0.4f;
    private static float numberScale = 55;
    private Sprite shapeSprite;
    private Sprite addonPicture;
    private AudioClip shapeClip;
    private static Sprite emptySprite;
    private List<Path> paths = new List<Path>();
    private static Dictionary<string, Sprite> directions;
    private static UnityEngine.Object[] numbersSprites;
    private static List<ShapesManager> shapesManagers;
    private List<int> SMShapesIndexes = new List<int>();
    private static string[] shapesManagersNames;
    private string[] positionsInshapeManagerNames;

    [MenuItem("Tools/English Tracing Book/Game Scene/Shapes Factory #f", false, 0)]
    static void ManagePipes()
    {
        Init();
    }

    [MenuItem("Tools/English Tracing Book/Game Scene/Shapes Factory #f", true, 0)]
    static bool ManagePipesValidate()
    {
        return !Application.isPlaying && SceneManager.GetActiveScene().name == "Game";
    }

    public static void Init()
    {
        if (directions != null)
            directions.Clear();
        else
            directions = new Dictionary<string, Sprite>();

        //Load Empty sprite
        emptySprite = Resources.Load("Textures/Empty" , typeof(Sprite)) as Sprite;

        //Load Numbers Sprites
        numbersSprites = Resources.LoadAll("Textures/Numbers");


        //Load directions sprites from Resources folder
		Array shapeTypeEnum = Enum.GetValues (typeof(Path.ShapeType));
        foreach (Path.ShapeType st in shapeTypeEnum)
        {
            Sprite sprite = Resources.Load("Textures/Editor/"+st.ToString(), typeof(Sprite)) as Sprite;

            if (sprite != null)
            {
                directions.Add(st.ToString(), sprite);
            }
        }

       shapesManagers = GameObject.FindObjectOfType<SingletonManager>().GetShapesManagers(new string[] { "SShapesManager" });
       shapesManagersNames = new string[shapesManagers.Count];
        for (int i = 0; i < shapesManagers.Count; i++)
        {
            shapesManagersNames[i] = shapesManagers[i].name;
        }

        window = (ShapeFactoryEditor)EditorWindow.GetWindow(typeof(ShapeFactoryEditor));
        float windowSize = Screen.currentResolution.height * 0.75f;
        window.position = new Rect(50, 100, windowSize, windowSize);
        window.maximized = false;
        window.titleContent.text = "Shape Factory";
        window.Show();
    }

    void OnGUI()
    {
        if (window == null || Application.isPlaying)
        {
            return;
        }

        window.Repaint();

        scrollView = new Vector2(position.width, position.height);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(scrollView.x), GUILayout.Height(scrollView.y));

        ShowInstructions();
        ShowShapeSection();
        ShowPathsSection();
        ShowAddTopShapesManagerSection();
        ShowCreateShapeSection();
        ShowAuthorSection();

        EditorGUILayout.EndScrollView();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    private void ShowInstructions()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        showInstructions = EditorGUILayout.Foldout(showInstructions, "Instructions");
        EditorGUILayout.Separator();
        if (showInstructions)
        {
            EditorGUILayout.HelpBox("Set the 'Sprite/Scale' of the shape.", MessageType.Info);
            EditorGUILayout.HelpBox("Set the 'Addon Picture/Clip' of the shape.", MessageType.Info);
            EditorGUILayout.HelpBox("Click On 'Plus/Minus' button to create/delete path.", MessageType.Info);
            EditorGUILayout.HelpBox("Edit the paths of the shape.", MessageType.Info);
            EditorGUILayout.HelpBox("Click On 'Create Shape' button to create the shape.", MessageType.Info);
        }
    }

    private void ShowShapeSection()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.Separator();
        shapeScale = EditorGUILayout.Slider("Shape Scale", shapeScale, 0.1f, 10);
        EditorGUILayout.Separator();
        shapeSprite = EditorGUILayout.ObjectField("Shape Sprite", shapeSprite, typeof(Sprite), true) as Sprite;
        EditorGUILayout.Separator();
        addonPicture = EditorGUILayout.ObjectField("Addon Picture", addonPicture, typeof(Sprite), true) as Sprite;
        EditorGUILayout.Separator();
        shapeClip = EditorGUILayout.ObjectField("Shape Clip", shapeClip, typeof(AudioClip), true) as AudioClip;
        EditorGUILayout.Separator();
        EditorGUILayout.EndVertical();
    }

    private void ShowPathsSection()
    {
        EditorGUILayout.HelpBox("Define the paths of the Shape below. Click on plus button to create a new path. Click on the minus button to remove the last path.", MessageType.Info);
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Colors.greenColor;
        if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
        {

            Sprite number1 = null, number2 = null;

            if (numbersSprites.Length > paths.Count * 2 + 1)
            {
                number1 = numbersSprites[paths.Count * 2 + 1] as Sprite;
                number2 = numbersSprites[paths.Count * 2 + 2] as Sprite;
            }
            else
            {
                Debug.LogWarning("You need to add more sprites for the Numbers image in <b>English Tracing Book/Resources/Textures/Numbers.png</b>");
            }

            paths.Add(new Path(number1, number2));
        }
        GUI.backgroundColor = Colors.whiteColor;

        GUI.backgroundColor = Colors.redColor;
        if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
        {
            if (paths.Count > 0)
            {
                paths.RemoveAt(paths.Count - 1);
            }
        }
        GUI.backgroundColor = Colors.whiteColor;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        if (paths.Count != 0)
        {
            numberScale = EditorGUILayout.Slider("Numbers Scale", numberScale, 1, 100);
            EditorGUILayout.Separator();
        }
        EditorGUILayout.LabelField("You have added ("+paths.Count +")"+ " Paths",EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical();
        for (int i = 0; i < paths.Count; i++)
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Path [" + i + "]", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUI.BeginDisabledGroup(i == paths.Count - 1);
            if (GUILayout.Button("▼", GUILayout.Width(30), GUILayout.Height(30)))
            {
                MoveDown(i);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(i - 1 < 0);
            if (GUILayout.Button("▲", GUILayout.Width(30), GUILayout.Height(30)))
            {
                MoveUp(i);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            paths[i].sprite = EditorGUILayout.ObjectField("Sprite", paths[i].sprite, typeof(Sprite)) as Sprite;
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            paths[i].enableNumber1 = EditorGUILayout.Toggle("", paths[i].enableNumber1, GUILayout.Width(12));

            if (!paths[i].enableNumber1)
                EditorGUI.BeginDisabledGroup(true);
            paths[i].number1 = EditorGUILayout.ObjectField("First Number", paths[i].number1, typeof(Sprite)) as Sprite;
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            paths[i].enableNumber2 = EditorGUILayout.Toggle("", paths[i].enableNumber2, GUILayout.Width(12));

            if (!paths[i].enableNumber2)
                EditorGUI.BeginDisabledGroup(true);
            paths[i].number2 = EditorGUILayout.ObjectField("Second Number", paths[i].number2, typeof(Sprite)) as Sprite;
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            Sprite directionSprite = null;
            directions.TryGetValue(paths[i].type.ToString(), out directionSprite);

            EditorGUILayout.BeginVertical();
            GUILayout.Space(35);
            paths[i].type = (Path.ShapeType)EditorGUILayout.EnumPopup("Type", paths[i].type);
            EditorGUILayout.EndVertical();

            if (directionSprite != null)
            {
                GUI.backgroundColor = Colors.transparent;
                if (GUILayout.Button(directionSprite.texture, GUILayout.Width(80), GUILayout.Height(80)))
                {
                    if (paths[i].type == Path.ShapeType.HORIZONTAL_LEFT_TO_RIGHT)
                    {
                        paths[i].type = Path.ShapeType.HORIZONTAL_RIGHT_TO_LEFT;
                    }
                    else if (paths[i].type == Path.ShapeType.HORIZONTAL_RIGHT_TO_LEFT)
                    {
                        paths[i].type = Path.ShapeType.HORIZONTAL_LEFT_TO_RIGHT;
                    }
                    else if (paths[i].type == Path.ShapeType.VERTICAL_BOTTOM_TO_TOP)
                    {
                        paths[i].type = Path.ShapeType.VERTICAL_TOP_TO_BOTTOM;
                    }
                    else if (paths[i].type == Path.ShapeType.VERTICAL_TOP_TO_BOTTOM)
                    {
                        paths[i].type = Path.ShapeType.VERTICAL_BOTTOM_TO_TOP;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_BOTTOM)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_CLOCKWISE_LEFT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_LEFT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_CLOCKWISE_TOP;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_TOP)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_CLOCKWISE_RIGHT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_RIGHT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_CLOCKWISE_BOTTOM;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_BOTTOM)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_LEFT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_LEFT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_TOP;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_TOP)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_RIGHT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_RIGHT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_BOTTOM;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_TOP_LEFT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_CLOCKWISE_TOP_RIGHT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_TOP_RIGHT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_CLOCKWISE_BOTTOM_RIGHT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_BOTTOM_RIGHT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_CLOCKWISE_BOTTOM_LEFT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_BOTTOM_LEFT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_CLOCKWISE_TOP_LEFT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_TOP_LEFT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_LEFT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_LEFT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_RIGHT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_RIGHT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_TOP_RIGHT;
                    }
                    else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_TOP_RIGHT)
                    {
                        paths[i].type = Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_TOP_LEFT;
                    }
                    
                }
                GUI.backgroundColor = Colors.whiteColor;

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (paths[i].type != Path.ShapeType.UNDEFINED)
            {
                paths[i].completeOffset = EditorGUILayout.Slider("Complete Offset", paths[i].completeOffset, 0, 1);
                EditorGUILayout.Separator();
            }
          
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    private void ShowAddTopShapesManagerSection()
    {
        EditorGUILayout.Separator();

        if (shapesManagers.Count != 0)
        {
            if (previousSelectedShapesManager != selectedShapesManager){

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

            selectedShapesManager = EditorGUILayout.Popup("Shapes Manager", selectedShapesManager, shapesManagersNames);
            positionInSelectedShapesManager = EditorGUILayout.Popup("Position", positionInSelectedShapesManager, positionsInshapeManagerNames);

            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));
        }
    }

    private void ShowCreateShapeSection()
    {
        EditorGUILayout.Separator();
        GUI.backgroundColor = Colors.greenColor;
        if (GUILayout.Button("Create Shape", GUILayout.Width(120), GUILayout.Height(25)))
        {
            CreateShape();
        }
    }

    private void ShowAuthorSection()
    {
        GUI.backgroundColor = Colors.whiteColor;

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(position.width / 2 - 100);
        GUI.backgroundColor = Color.clear;
        if (GUILayout.Button("Developed by Indie Studio\nwww.indiestd.com", GUILayout.Width(200), GUILayout.Height(30)))
        {
            Application.OpenURL(Links.indieStudioStoreURL);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void CreateShape()
    {

        string message = "";
        string shapePrefabLocalPath = "";

        if (shapeSprite == null)
        {
            message += "- Shape sprite is not defined\n";
        }

        if (paths.Count == 0)
        {
            message += "- You need to define the paths of the shape\n";
        }

        if (!string.IsNullOrEmpty(message))
        {
            EditorUtility.DisplayDialog("Error Message", message, "ok");
            return;
        }
        else
        {
            shapePrefabLocalPath = EditorUtility.OpenFolderPanel("Create Shape", Application.dataPath + "/English Tracing Book/Prefabs", "");

            if (string.IsNullOrEmpty(shapePrefabLocalPath))
            {
                return;
            }

            bool isOk = EditorUtility.DisplayDialog("Create New Shape", "Shape Factory will delete all shapes under Shape gameobject in the Hirerachy of the Game scene and then create your new shape, Are you sure ?", "ok", "cancel");
            if (!isOk)
            {
                return;
            }
        }

        if (string.IsNullOrEmpty(shapePrefabLocalPath))
        {
            return;
        }

         //Validate Shape Parent
        GameObject shapeParent = GameObject.Find("Shape");

        if (shapeParent == null)
        {
            shapeParent = new GameObject("Shape");
            shapeParent.transform.SetParent(GameObject.Find("UICanvas").transform);
            shapeParent.transform.localPosition = Vector3.zero;
            shapeParent.transform.localScale = Vector3.one;

            RectTransform shapeParentRect = shapeParent.AddComponent<RectTransform>();
            shapeParentRect.anchorMin = new Vector2(0, 0);
            shapeParentRect.anchorMax = new Vector2(1, 1);
            shapeParentRect.offsetMin = shapeParentRect.offsetMax = Vector2.zero;
        }

        //Destory all current children
        foreach (Transform child in shapeParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        //Shape GameObject
        GameObject shapeGO = new GameObject(shapeSprite.name+"-shape");
        shapeGO.transform.SetParent(shapeParent.transform);
        shapeGO.transform.position = Vector3.zero;
        shapeGO.transform.localPosition = Vector3.zero;
        shapeGO.transform.localScale = Vector3.one;
        shapeGO.transform.localPosition = Vector3.zero;

        Shape shapeComponent = shapeGO.AddComponent<Shape>();

        RectTransform shapeRectTransform = shapeGO.AddComponent<RectTransform>();
        shapeRectTransform.sizeDelta = new Vector2(shapeSprite.texture.width, shapeSprite.texture.height);

        //Content GameObject
        GameObject shapeContentGO = new GameObject("Content");
        shapeContentGO.transform.SetParent(shapeGO.transform);
        shapeContentGO.transform.localPosition = Vector3.zero;
        shapeContentGO.transform.localScale = shapeScale * Vector3.one;

        RectTransform shapeContentRect = shapeContentGO.AddComponent<RectTransform>();
        shapeContentRect.anchorMin = new Vector2(0, 0);
        shapeContentRect.anchorMax = new Vector2(1, 1);
        shapeContentRect.offsetMin = shapeContentRect.offsetMax = Vector2.zero;

        Image shapeContentComp = shapeContentGO.AddComponent<Image>();
        shapeContentComp.sprite = shapeSprite;
        shapeContentComp.preserveAspect = true;
        shapeContentComp.raycastTarget = false;

        Animator shapeAnimator = shapeGO.AddComponent<Animator>();
        shapeAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/Shape");
        shapeComponent.animator = shapeAnimator;

        Shadow shapeShadow = shapeContentGO.AddComponent<Shadow>();
        shapeShadow.effectColor = new Color(0, 0, 0, 15) / 255.0f;
        shapeShadow.effectDistance = new Vector2(-8, -1);

        //Collider GameObject
        GameObject colliderGO = new GameObject("Collider");
        colliderGO.tag = "Collider";
        colliderGO.transform.SetParent(shapeContentGO.transform);
        colliderGO.transform.localPosition = Vector3.zero;
        colliderGO.transform.localScale = Vector3.one * shapeSprite.pixelsPerUnit;

        SpriteRenderer colliderSpriteR = colliderGO.AddComponent<SpriteRenderer>();
        colliderSpriteR.sprite = shapeSprite;
        PolygonCollider2D polygonCol = colliderGO.AddComponent<PolygonCollider2D>();
        polygonCol.isTrigger = true;
        DestroyImmediate(colliderSpriteR);

        //Create Paths
        GameObject pathsGO = new GameObject("Paths");
        pathsGO.transform.SetParent(shapeContentGO.transform);
        pathsGO.transform.localPosition = Vector3.zero;
        pathsGO.transform.localScale = Vector3.one;
        RectTransform pathsRect = pathsGO.AddComponent<RectTransform>();
        pathsRect.anchorMin = new Vector2(0, 0);
        pathsRect.anchorMax = new Vector2(1, 1);
        pathsRect.offsetMin = pathsRect.offsetMax = Vector2.zero;
       
        for (int i = 0; i < paths.Count; i++)
        {
            //Create New Path
            GameObject pathGO = new GameObject("Path-" + (i + 1) + "-" + (i + 2));
            pathGO.tag = "Path";
            pathGO.transform.SetParent(pathsGO.transform);
            pathGO.transform.localPosition = Vector3.zero;
            pathGO.transform.localScale = Vector3.one;
            RectTransform pathRect = pathGO.AddComponent<RectTransform>();
            pathRect.anchorMin = new Vector2(0, 0);
            pathRect.anchorMax = new Vector2(1, 1);
            pathRect.offsetMin = pathRect.offsetMax = Vector2.zero;
          
            //Create Path Fill
            GameObject pathFillGO = new GameObject("Fill-" + (i + 1) + "-" + (i + 2));
            pathFillGO.tag = "Fill";
            pathFillGO.transform.SetParent(pathGO.transform);
            pathFillGO.transform.localPosition = Vector3.zero;
            pathFillGO.transform.localScale = Vector3.one;
            RectTransform pathFillRect = pathFillGO.AddComponent<RectTransform>();
            pathFillRect.anchorMin = new Vector2(0, 0);
            pathFillRect.anchorMax = new Vector2(1, 1);
            pathFillRect.offsetMin = pathFillRect.offsetMax = Vector2.zero;
       
            Image pathFillImage = pathFillGO.AddComponent<Image>();
            pathFillImage.preserveAspect = true;
            pathFillImage.raycastTarget = false;
            pathFillImage.sprite = paths[i].sprite;
            pathFillImage.type = Image.Type.Filled;

            TracingPath tracingPath = pathGO.AddComponent<TracingPath>();
            tracingPath.fillImage = pathFillImage;

            tracingPath.completeOffset = paths[i].completeOffset;

            if (paths[i].type == Path.ShapeType.HORIZONTAL_LEFT_TO_RIGHT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Linear;
                pathFillImage.fillMethod = Image.FillMethod.Horizontal;
                pathFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            }
            else if (paths[i].type == Path.ShapeType.HORIZONTAL_RIGHT_TO_LEFT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Linear;
                pathFillImage.fillMethod = Image.FillMethod.Horizontal;
                pathFillImage.fillOrigin = (int)Image.OriginHorizontal.Right;
            }
            else if (paths[i].type == Path.ShapeType.VERTICAL_BOTTOM_TO_TOP)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Linear;
                pathFillImage.fillMethod = Image.FillMethod.Vertical;
                pathFillImage.fillOrigin = (int)Image.OriginVertical.Bottom;
            }
            else if (paths[i].type == Path.ShapeType.VERTICAL_TOP_TO_BOTTOM)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Linear;
                pathFillImage.fillMethod = Image.FillMethod.Vertical;
                pathFillImage.fillOrigin = (int)Image.OriginVertical.Top;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_LEFT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Left;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_RIGHT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Right;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_TOP)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Top;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_CLOCKWISE_BOTTOM)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Bottom;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_LEFT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Left;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_RIGHT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Right;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_TOP)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Top;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_360_COUNTER_CLOCKWISE_BOTTOM)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial360;
                pathFillImage.fillOrigin = (int)Image.Origin360.Bottom;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_BOTTOM_LEFT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.BottomLeft;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_BOTTOM_RIGHT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.BottomRight;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_TOP_LEFT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.TopLeft;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_CLOCKWISE_TOP_RIGHT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.TopRight;
                pathFillImage.fillClockwise = true;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_LEFT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.BottomLeft;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_RIGHT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.BottomRight;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_TOP_LEFT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.TopLeft;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.RADIAL_90_COUNTER_CLOCKWISE_TOP_RIGHT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Radial;
                pathFillImage.fillMethod = Image.FillMethod.Radial90;
                pathFillImage.fillOrigin = (int)Image.Origin90.TopRight;
                pathFillImage.fillClockwise = false;
            }
            else if (paths[i].type == Path.ShapeType.POINT)
            {
                tracingPath.fillMethod = TracingPath.FillMethod.Point;
            }

            shapeComponent.tracingPaths.Add(tracingPath);
            pathFillImage.fillAmount = 0;

            //Create Numbers
            Color numberShadowColor = Color.white;
            numberShadowColor.a = 0.2f;

            GameObject number1 = new GameObject("FirstNumber");
            number1.tag = "Number";
            number1.transform.SetParent(pathGO.transform);
            number1.transform.localPosition = Vector3.zero;
            number1.transform.localScale = Vector3.one;
            RectTransform number1Rect = number1.AddComponent<RectTransform>();
            number1Rect.sizeDelta = new Vector2(numberScale, numberScale);
            number1Rect.SetSiblingIndex(pathFillRect.GetSiblingIndex() + 1);
            Image number1Image = number1.AddComponent<Image>();
            number1Image.preserveAspect = true;
            number1Image.raycastTarget = false;
            number1Image.sprite = paths[i].number1;
            if (paths[i].enableNumber1) number1Image.enabled = true; else number1Image.enabled = false;

            Animator number1Animator = number1.AddComponent<Animator>();
            number1Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/Number");
            Shadow number1Shadow = number1.AddComponent<Shadow>();
            number1Shadow.effectColor = numberShadowColor;

            GameObject number2 = new GameObject("SecondNumber");
            number2.tag = "Number";
            number2.transform.SetParent(pathGO.transform);
            number2.transform.localPosition = Vector3.zero;
            number2.transform.localScale = Vector3.one;
            RectTransform number2Rect = number2.AddComponent<RectTransform>();
            number2Rect.sizeDelta = new Vector2(numberScale, numberScale);
            number2Rect.SetSiblingIndex(pathFillRect.GetSiblingIndex() + 2);
            Image number2Image = number2.AddComponent<Image>();
            number2Image.preserveAspect = true;
            number2Image.raycastTarget = false;
            number2Image.sprite = paths[i].number2;
            if (paths[i].enableNumber2) number2Image.enabled = true; else number2Image.enabled = false;

            Animator number2Animator = number2.AddComponent<Animator>();
            number2Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/Number");
            Shadow number2Shadow = number2.AddComponent<Shadow>();
            number2Shadow.effectColor = numberShadowColor;

            tracingPath.firstNumber = number1.transform;
            tracingPath.secondNumber = number2.transform;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene()); 

        shapePrefabLocalPath = "Assets" + shapePrefabLocalPath.Replace(Application.dataPath, "");

        GameObject savedShapePrefab = CommonUtil.SaveAsPrefab(shapePrefabLocalPath + "/" + shapeGO.name + ".prefab", shapeGO, false);

        if (addToShapesManager && savedShapePrefab!=null)
        {
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
                index = SMShapesIndexes[positionInSelectedShapesManager-1]+1;// After this shape
            }

            shapesManagers[selectedShapesManager].shapes.Insert(index, new ShapesManager.Shape() { prefab = savedShapePrefab, clip = shapeClip, picture = addonPicture});
            shapesManagers[selectedShapesManager].RemoveEmptyShapes();
            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.OpenScene("Assets/English Tracing Book/Scenes/Main.unity");
            CommonUtil.ReplacePrefab(GameObject.Find(shapesManagers[selectedShapesManager].name), shapesManagers[selectedShapesManager]);
            EditorSceneManager.OpenScene("Assets/English Tracing Book/Scenes/Game.unity");
        }

        Selection.activeGameObject = GameObject.Find("Shape").transform.GetChild(0).transform.Find("Content").Find("Paths").Find("Path-1-2").gameObject;
        EditorUtility.DisplayDialog("Successful Message", "Shape is created sucessfully , Now you must create the Curve for each Path using Path Component", "ok");

        window.Close();
    }

    private void MoveUp(int index)
    {
       Path path = paths[index];
       paths.RemoveAt(index);
       paths.Insert(index - 1, path);
    }

    private void MoveDown(int index)
    {
        Path path = paths[index];
        paths.RemoveAt(index);
        paths.Insert(index + 1, path);
    }

    private class Path
    {
        public Sprite sprite;
        public ShapeType type;
        public float completeOffset = 0.85f;
        public Sprite number1, number2;
        public bool enableNumber1 = true, enableNumber2 = true;

        public Path()
        {
        }

        public Path(Sprite number1,Sprite number2)
        {
            if(emptySprite!=null)
            this.sprite = emptySprite;

            this.number1 = number1;
            this.number2 = number2;
        }

        public enum ShapeType
        {
            UNDEFINED,
            HORIZONTAL_RIGHT_TO_LEFT,
            HORIZONTAL_LEFT_TO_RIGHT,
            VERTICAL_TOP_TO_BOTTOM,
            VERTICAL_BOTTOM_TO_TOP,
            RADIAL_360_CLOCKWISE_LEFT,
            RADIAL_360_CLOCKWISE_RIGHT,
            RADIAL_360_CLOCKWISE_TOP,
            RADIAL_360_CLOCKWISE_BOTTOM,
            RADIAL_360_COUNTER_CLOCKWISE_LEFT,
            RADIAL_360_COUNTER_CLOCKWISE_RIGHT,
            RADIAL_360_COUNTER_CLOCKWISE_TOP,
            RADIAL_360_COUNTER_CLOCKWISE_BOTTOM,
            RADIAL_90_CLOCKWISE_TOP_LEFT,
            RADIAL_90_CLOCKWISE_TOP_RIGHT,
            RADIAL_90_CLOCKWISE_BOTTOM_RIGHT,
            RADIAL_90_CLOCKWISE_BOTTOM_LEFT,
            RADIAL_90_COUNTER_CLOCKWISE_TOP_LEFT,
            RADIAL_90_COUNTER_CLOCKWISE_TOP_RIGHT,
            RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_RIGHT,
            RADIAL_90_COUNTER_CLOCKWISE_BOTTOM_LEFT,
            POINT
        }
    }
}