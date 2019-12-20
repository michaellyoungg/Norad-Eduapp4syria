using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
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
    #pragma warning disable 0168 // variable declared but not used.
    #pragma warning disable 0219 // variable assigned but not used.

    [DisallowMultipleComponent]
    public class ShapesTable : MonoBehaviour
    {
        /// <summary>
        /// Whether to create groups pointers or not.
        /// </summary>
        public bool createGroupsPointers = true;

        /// <summary>
        /// Whether to save the last selected group or not.
        /// </summary>
        public bool saveLastSelectedGroup = true;

        /// <summary>
        /// The groups parent.
        /// </summary>
        public Transform groupsParent;

        /// <summary>
        /// The pointers parent.
        /// </summary>
        public Transform pointersParent;

        /// <summary>
        /// The collected stars text.
        /// </summary>
        public Text collectedStarsText;

        /// <summary>
        /// The shape bright.
        /// </summary>
        public Transform shapeBright;

        /// <summary>
        /// The star on sprite.
        /// </summary>
        public Sprite starOn;

        /// <summary>
        /// The star off sprite.
        /// </summary>
        public Sprite starOff;

        /// <summary>
        /// The line prefab
        /// </summary>
        public GameObject linePrefab;

        /// <summary>
        /// The table shape prefab.
        /// </summary>
        public GameObject shapePrefab;

        /// <summary>
        /// The shapes group prefab.
        /// </summary>
        public GameObject shapesGroupPrefab;

        /// <summary>
        /// The pointer prefab.
        /// </summary>
        public GameObject pointerPrefab;

        /// <summary>
        /// temporary transform.
        /// </summary>
        private Transform tempTransform;

        /// <summary>
        /// The loading reference.
        /// </summary>
        public GameObject loading;

        /// <summary>
        /// The Number of shapes per group.
        /// </summary>
        [Range(1, 100)]
        public int shapesPerGroup = 12;

        /// <summary>
        /// Number of columns per group.
        /// </summary>
        [Range(1, 10)]
        public int columnsPerGroup = 3;

        /// <summary>
        /// Whether to enable group grid layout.
        /// </summary>
        public bool EnableGroupGridLayout = true;

        /// <summary>
        /// The album title.
        /// </summary>
        public Image albumTitle;

        /// <summary>
        /// The last shape that user reached.
        /// </summary>
        private Transform lastShape;

        /// <summary>
        /// A static instance of this class
        /// </summary>
        public static ShapesTable instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            if (string.IsNullOrEmpty(ShapesManager.shapesManagerReference))
            {
                Debug.LogError("You have to start the game from the Main scene");
                return;
            }

            if (albumTitle != null)
                albumTitle.sprite = ShapesManager.GetCurrentShapesManager().albumTitle;

            //Create new shapes
            StartCoroutine("CreateShapes");

            //Setup the last selected group index
            /*
            ScrollSlider scrollSlider = GameObject.FindObjectOfType<ScrollSlider> ();
            if (saveLastSelectedGroup && ShapesManager.GetCurrentShapesManager() != null) {
                scrollSlider.currentGroupIndex = ShapesManager.GetCurrentShapesManager().lastSelectedGroup;
            }*/
        }

        void Update()
        {
            if (lastShape != null)
            {
                //Set the bright postion to the last shape postion
                if (!Mathf.Approximately(lastShape.position.magnitude, shapeBright.position.magnitude))
                {
                    shapeBright.position = lastShape.position;
                }
            }
        }


        /// <summary>
        /// Creates the shapes in Groups.
        /// </summary>
        private IEnumerator CreateShapes()
        {
            yield return 0;

            //The ID of the shape
            int ID = 0;

            //The group of the shape
            GameObject shapesGroup = null;

            //The index of the group
            int groupIndex = 0;

            pointersParent.gameObject.SetActive(false);
            groupsParent.gameObject.SetActive(false);

            List<TracingPath> tracingPathsList = new List<TracingPath>();

            //Create Shapes inside groups
            for (int i = 0; i < ShapesManager.GetCurrentShapesManager().shapes.Count; i++)
            {

                if (ShapesManager.GetCurrentShapesManager().shapes[i].prefab == null)
                {
                    Debug.LogWarning("An empty shape's prefab is found in " + ShapesManager.GetCurrentShapesManager().name);
                    continue;
                }

                if (i % shapesPerGroup == 0)
                {
                    groupIndex = (i / shapesPerGroup);
                    shapesGroup = Group.CreateGroup(shapesGroupPrefab, groupsParent, groupIndex, columnsPerGroup);
                    if (!EnableGroupGridLayout)
                    {
                        shapesGroup.GetComponent<GridLayoutGroup>().enabled = false;
                    }
                    if (createGroupsPointers)
                    {
                        Pointer.CreatePointer(groupIndex, shapesGroup, pointerPrefab, pointersParent);
                    }
                }

                //Create new table Shape
                ID = (i);//the id of the shape
                GameObject tableShapeGameObject = Instantiate(shapePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                tableShapeGameObject.transform.SetParent(shapesGroup.transform);//setting up the shape's parent
                TableShape tableShapeComponent = tableShapeGameObject.GetComponent<TableShape>();//get TableShape Component
                tableShapeComponent.ID = ID;//setting up shape ID
                tableShapeGameObject.name = "Shape-" + ID;//shape name
                tableShapeGameObject.transform.localScale = Vector3.one;
                tableShapeGameObject.transform.localPosition = Vector3.zero;
                tableShapeGameObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
                tableShapeGameObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;

                Transform content = tableShapeGameObject.transform.Find("Content");
                GameObject uiShape = Instantiate(ShapesManager.GetCurrentShapesManager().shapes[i].prefab, Vector3.zero, Quaternion.identity) as GameObject;
                uiShape.transform.SetParent(content);
                uiShape.name = ShapesManager.GetCurrentShapesManager().shapes[i].prefab.name;
                uiShape.transform.localScale = new Vector3(CommonUtil.ShapeAlbumAspectRatio() * ShapesManager.GetCurrentShapesManager().albumShapeScaleFactor, CommonUtil.ShapeAlbumAspectRatio() * ShapesManager.GetCurrentShapesManager().albumShapeScaleFactor);

                uiShape.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

                List<Shape> shapeComponents = new List<Shape>();
                if (uiShape.GetComponent<CompoundShape>() != null)
                {
                    //sentence
                    Shape[] tempS = uiShape.GetComponentsInChildren<Shape>();
                    foreach (Shape s in tempS)
                    {
                        shapeComponents.Add(s);
                    }
                }
                else
                {
                    //single shape
                    shapeComponents.Add(uiShape.GetComponent<Shape>());
                }

                int compoundID;
                for (int s = 0; s < shapeComponents.Count; s++)
                {
                    CompoundShape compundShape = shapeComponents[s].transform.parent.GetComponent<CompoundShape>();
                    Transform shapeContent = shapeComponents[s].transform.Find("Content");

                    compoundID = -1;
                    if (compundShape != null)
                    {
                        compoundID = compundShape.GetShapeIndexByInstanceID(shapeComponents[s].GetInstanceID());
                    }

                    if (ShapesManager.GetCurrentShapesManager().shapes[ID].starsNumber != ShapesManager.Shape.StarsNumber.ZERO)
                    {
                        //Hide shape image whene user has not zero stars
                        //shapeContent.GetComponent<Image>().enabled = false;
                    }

                    shapeComponents[s].enabled = false;

                    //Release unwanted resources
                    shapeComponents[s].transform.GetComponent<Animator>().enabled = false;
                    shapeContent.Find("Collider").gameObject.SetActive(false);

                    int from, to;
                    string[] slices;
                    foreach (TracingPath p in shapeComponents[s].tracingPaths)
                    {
                        TracingPath tracingPath = p.GetComponent<TracingPath>();

                        if (p.curve == null)
                        {
                            Debug.LogError("Curve is not defined in " + p.name + " inside " + shapeComponents[s].name);
                            continue;
                        }
                        slices = p.name.Split('-');
                        from = int.Parse(slices[1]);
                        to = int.Parse(slices[2]);

                        if (PlayerPrefs.HasKey(DataManager.GetPathStrKey(ID, compoundID, from, to, ShapesManager.GetCurrentShapesManager())))
                        {

                            //Hide the numbers of the filled path
                            p.firstNumber.gameObject.SetActive(false);
                            p.secondNumber.gameObject.SetActive(false);

                            Color pathColor = DataManager.GetShapePathColor(ID, compoundID, from, to, ShapesManager.GetCurrentShapesManager());

                            if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.FILL)
                            {
                                p.curve.gameObject.SetActive(false);
                                p.fillImage.fillAmount = 1;
                                p.fillImage.color = pathColor;
                            }
                            else if (ShapesManager.GetCurrentShapesManager().tracingMode == ShapesManager.TracingMode.LINE)
                            {
                                tracingPathsList.Add(tracingPath);
                                tracingPath.line.SetColor(CommonUtil.ColorToGradient(pathColor));
                            }
                        }
                    }
                }

                tableShapeGameObject.GetComponent<Button>().onClick.AddListener(() => UIEvents.instance.AlbumShapeEvent(tableShapeGameObject.GetComponent<TableShape>()));
                //Setting up the shape contents (stars number ,islocked,...)
                SettingUpTableShape(ShapesManager.GetCurrentShapesManager().shapes[ID], tableShapeComponent, ID, groupIndex);
            }

            collectedStarsText.text = ShapesManager.GetCurrentShapesManager().GetCollectedStars() + "/" + (3 * ShapesManager.GetCurrentShapesManager().shapes.Count);

            if (ShapesManager.GetCurrentShapesManager().shapes.Count == 0)
            {
                Debug.Log("There are no Shapes found");
            }
            else
            {
                Debug.Log("New shapes have been created");
            }

            loading.SetActive(false);

            pointersParent.gameObject.SetActive(true);
            groupsParent.gameObject.SetActive(true);

            foreach (TracingPath tp in tracingPathsList)
            {
                tp.SetUpLineWidth();
                tp.OnComplete();
            }

            ScrollSlider.instance.Init();
        }

        /// <summary>
        /// Settings up the table shape contents in the table.
        /// </summary>
        /// <param name="shape">ShapesManager shape reference.</param>
        /// <param name="tableShape">Table shape reference.</param>
        /// <param name="ID">ID of the shape.</param>
        /// <param name="groupIndex">Index of the group.</param>
        private void SettingUpTableShape(ShapesManager.Shape shape, TableShape tableShape, int ID, int groupIndex)
        {
            if (tableShape == null)
            {
                return;
            }

            if (!shape.isLocked || ShapesManager.GetCurrentShapesManager().testMode)
            {
                tableShape.transform.Find("Cover").gameObject.SetActive(false);
                tableShape.transform.Find("Lock").gameObject.SetActive(false);
            }
            else
            {
                tableShape.GetComponent<Button>().interactable = false;
                tableShape.transform.Find("Stars").gameObject.SetActive(false);
            }

            //Set Last reached shape
            if (!ShapesManager.GetCurrentShapesManager().testMode)
                if (!shape.isLocked)
                {
                    if (PlayerPrefs.HasKey(DataManager.GetLockedStrKey(ID + 1, ShapesManager.GetCurrentShapesManager())))
                    {
                        if (DataManager.IsShapeLocked(ID + 1, ShapesManager.GetCurrentShapesManager()))
                        {
                            SetSelectedGroup(groupIndex);
                        }
                    }
                    else if (!PlayerPrefs.HasKey(DataManager.GetStarsStrKey(ID, ShapesManager.GetCurrentShapesManager())))
                    {
                        SetSelectedGroup(groupIndex);
                    }
                }

            tempTransform = tableShape.transform.Find("Stars");

            //Apply the current Stars Rating 
            if (shape.starsNumber == ShapesManager.Shape.StarsNumber.ONE)
            {//One Star
                tempTransform.Find("FirstStar").GetComponent<Image>().sprite = starOn;
                tempTransform.Find("SecondStar").GetComponent<Image>().sprite = starOff;
                tempTransform.Find("ThirdStar").GetComponent<Image>().sprite = starOff;
            }
            else if (shape.starsNumber == ShapesManager.Shape.StarsNumber.TWO)
            {//Two Stars
                tempTransform.Find("FirstStar").GetComponent<Image>().sprite = starOn;
                tempTransform.Find("SecondStar").GetComponent<Image>().sprite = starOn;
                tempTransform.Find("ThirdStar").GetComponent<Image>().sprite = starOff;
            }
            else if (shape.starsNumber == ShapesManager.Shape.StarsNumber.THREE)
            {//Three Stars
                tempTransform.Find("FirstStar").GetComponent<Image>().sprite = starOn;
                tempTransform.Find("SecondStar").GetComponent<Image>().sprite = starOn;
                tempTransform.Find("ThirdStar").GetComponent<Image>().sprite = starOn;
            }
            else
            {//Zero Stars
                tempTransform.Find("FirstStar").GetComponent<Image>().sprite = starOff;
                tempTransform.Find("SecondStar").GetComponent<Image>().sprite = starOff;
                tempTransform.Find("ThirdStar").GetComponent<Image>().sprite = starOff;
            }
        }

        /// <summary>
        /// Set the selected group.
        /// </summary>
        /// <param name="groupIndex">Group index.</param>
        private void SetSelectedGroup(int groupIndex)
        {
            //Setup the last selected group index
            ScrollSlider.instance.currentGroupIndex = groupIndex;
        }

        /// <summary>
        /// Raise the change group event.
        /// </summary>
        /// <param name="currentGroup">Current group.</param>
        public void OnChangeGroup(int currentGroup)
        {
            if (saveLastSelectedGroup)
            {
                ShapesManager.GetCurrentShapesManager().lastSelectedGroup = currentGroup;
            }
        }
    }
}