using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using IndieStudio.EnglishTracingBook.Utility;

/*
 * English Tracing Book Package
 *
 * @License		      Unity Asset Store EULA https://unity3d.com/legal/as_terms
 * @Author		      Indie Studio - Baraa Nasser
 * @Website		      https://indiestd.com
 * @Asset Store       https://www.assetstore.unity3d.com/en/#!/publisher/9268
 * @Unity Connect     https://connect.unity.com/u/5822191d090915001dbaf653/column
 * @Email		      info@indiestd.com
 *
 */

namespace IndieStudio.EnglishTracingBook.Game
{
    [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Whether the script is running or not.
        /// </summary>
        public bool isRunning = true;

        public ShapesManager mysm ;

        public Color linecolor;

        public bool running = true;

        public Camera clickcam;

        /// <summary>
        /// Whether to hide shape's image on complete or not
        /// </summary>
        public bool hideShapeImageOnComplete;

        /// <summary>
        /// Whether to move/animate numbers of the paths on Start or not
        /// </summary>
        public bool animateNumbersOnStart = true;

        /// <summary>
        /// Whether to enable Fill Qurater Angle Restriction or not
        /// </summary>
        private bool quarterRestriction = true;

        /// <summary>
        /// The line's prefab
        /// </summary>
        public GameObject linePrefab;

        /// <summary>
        /// The PathObjectMove of the Hand
        /// </summary>
        public PathObjectMove handPOM;

        /// <summary>
        /// The reset confirm dialog
        /// </summary>
        public Dialog resetConfirmDialog;

        /// <summary>
        /// The next button transform
        /// </summary>
        public Transform nextButton;

        /// <summary>
        /// The current pencil.
        /// </summary>
        public Pencil currentPencil;

        /// <summary>
        /// The shape order.
        /// </summary>
        public Text shapeOrder;

        /// <summary>
        /// The write shape name text.
        /// </summary>
        public Text writeText;

        /// <summary>
        /// The tracing path.
        /// </summary>
        private TracingPath tracingPath;

        /// <summary>
        /// The shape parent.
        /// </summary>
        public Transform shapeParent;

        /// <summary>
        /// The shape reference.
        /// </summary>
        [HideInInspector]
        public Shape shape;

        /// <summary>
        /// The click postion.
        /// </summary>
        private Vector3 clickPostion;

        /// <summary>
        /// The direction between click and shape.
        /// </summary>
        private Vector2 direction;

        /// <summary>
        /// The current angle , angleOffset and fill amount.
        /// </summary>
        private float angle, angleOffset, fillAmount;

        /// <summary>
        /// The clock wise sign.
        /// </summary>
        private float clockWiseSign;

        /// <summary>
        /// The hand reference.
        /// </summary>
        public Transform hand;

        /// <summary>
        /// The default size of the cursor.
        /// </summary>
        private Vector3 cursorDefaultSize;

        /// <summary>
        /// The click size of the cursor.
        /// </summary>
        private Vector3 cursorClickSize;

        /// <summary>
        /// The target quarter of the radial fill.
        /// </summary>
        private float targetQuarter;

        /// <summary>
        /// The complete effect.
        /// </summary>
        public ParticleSystem winEffect;

        /// <summary>
        /// The timer reference. 
        /// </summary>
        public Timer timer;

        /// <summary>
        /// The window dialog reference.
        /// </summary>
        public WinDialog winDialog;

        /// <summary>
        /// The shape picture image reference (used to show the picture image  of the selected shape).
        /// </summary>
        public Image shapePicture;

        /// <summary>
        /// The hit2d reference.
        /// </summary>
        private RaycastHit2D hit2d;

        /// <summary>
        /// The compound shape reference.
        /// </summary>
        [HideInInspector]
        public CompoundShape compoundShape;

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static GameManager instance;

        void Awake()
        {
            //Initiate GameManager instance 
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
           Init();
        }

        /// <summary>
        /// Init references/values
        /// </summary>
        public void Init()
        {
            ShapesManager.shapesManagerReference = "MShapesManager";
            //Initiate values and setup the references
            cursorDefaultSize = hand.transform.localScale;
            cursorClickSize = cursorDefaultSize / 1.2f;


          
            if (handPOM == null)
            {
                handPOM = GameObject.Find("TracingHand").GetComponent<PathObjectMove>();
            }

            

            winEffect.gameObject.SetActive(false);

            ResetTargetQuarter();
            SetShapeOrderColor();
            Debug.Log("about to create shape!");
            CreateShape();
        }

        IEnumerator LoadHomeScene()
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
            // a sceneBuildIndex of 1 as shown in Build Settings.

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Game Logic is here

            if (!isRunning)
            {
                return;
            }

            if (!running)
                return;

            HandleInput();
        }

        /// <summary>
        /// Handle user's input
        /// </summary>
        private void HandleInput()
        {
            DrawHand(GetCurrentPlatformClickPosition(clickcam));
            DrawBrightEffect(GetCurrentPlatformClickPosition(clickcam));

            if (shape == null)
            {
                return;
            }

            if (shape.completed)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                //if (!shape.completed)
                //brightEffect.gameObject.SetActive(false);

                hit2d = Physics2D.Raycast(GetCurrentPlatformClickPosition(clickcam), Vector2.zero);
                //Debug.Log("hitting " + GetCurrentPlatformClickPosition(clickcam));
                if (hit2d.collider != null)
                {
                    if (hit2d.transform.tag == "Point")
                    {
                        OnPointHitCollider(hit2d);

                        if (Input.GetMouseButtonDown(0))
                        {
                            shape.CancelInvoke();
                            DisableHandTracing();
                            ShowUserTouchHand();
                        }
                    }
                    else if (hit2d.transform.tag == "Collider")
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            DisableHandTracing();
                            ShowUserTouchHand();
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //brightEffect.gameObject.SetActive(false);
                HideUserTouchHand();
                StartAutoTracing(shape, 1);
                ResetPath();
            }

            if (tracingPath == null)
            {
                return;
            }

            if (tracingPath.completed)
            {
                return;
            }

            if (mysm.enableTracingLimit)
            {
                hit2d = Physics2D.Raycast(GetCurrentPlatformClickPosition(Camera.main), Vector2.zero);
                if (hit2d.collider == null)
                {
                    AudioSources.instance.PlayWrongSFX();
                    ResetPath();
                    return;
                }
            }

            TracePath();
        }

        /// <summary>
        /// On Point collider hit.
        /// </summary>
        /// <param name="hit2d">Hit2d.</param>
        private void OnPointHitCollider(RaycastHit2D hit2d)
        {
            tracingPath = hit2d.transform.GetComponentInParent<TracingPath>();

            if (tracingPath == null)
            {
                return;
            }

            tracingPath.DisableCurrentPoint();
            tracingPath.tracedPoints++;
            tracingPath.EnableCurrentPoint();

            if (tracingPath.completed || !shape.IsCurrentPath(tracingPath))
            {
                ReleasePath();
            }
            else
            {
                tracingPath.StopAllCoroutines();
                tracingPath.fillImage.color = linecolor;
            }

            tracingPath.curve.Init();

            if (!tracingPath.shape.enablePriorityOrder)
            {
                shape = tracingPath.shape;
            }
        }

        /// <summary>
        /// Get the current platform click position in the world space.
        /// </summary>
        /// <returns>The current platform click position.</returns>
        private Vector3 GetCurrentPlatformClickPosition(Camera camera)
        {
            Vector3 clickPosition = Vector3.zero;

            if (Application.isMobilePlatform)
            {//current platform is mobile
                if (Input.touchCount != 0)
                {
                    Touch touch = Input.GetTouch(0);
                    clickPosition = touch.position;
                }
            }
            else
            {//others
                clickPosition = Input.mousePosition;
            }

            clickPosition = camera.ScreenToWorldPoint(clickPosition);//get click position in the world space
            clickPosition.z = 0;
            return clickPosition;
        }

        public void gobackhome()
        {
            Debug.Log("Happening!");
            running = false;
            //  IndieStudio.EnglishTracingBook.Utility.SceneLoader.instance.LoadScene("LevelScene");
            StartCoroutine(IndieStudio.EnglishTracingBook.Utility.SceneLoader.instance.CanvasFade());
            StartCoroutine(LoadHomeScene());
        }
        /// <summary>
        /// Create new shape.
        /// </summary>
        private void CreateShape()
        {
            timer.Reset();
            winEffect.gameObject.SetActive(false);
            resetConfirmDialog.Hide(true);
            BlackArea.instance.Hide();
            winDialog.Hide();
            nextButton.GetComponent<Animator>().SetBool("Select", false);

            CompoundShape currentCompoundShape = GameObject.FindObjectOfType<CompoundShape>();
            if (currentCompoundShape != null)
            {
                DestroyImmediate(currentCompoundShape.gameObject);
            }
            else
            {
                Shape shapeComponent = GameObject.FindObjectOfType<Shape>();
                if (shapeComponent != null)
                {
                    DestroyImmediate(shapeComponent.gameObject);
                }
            }

            try
            {
               //shapeOrder.text = (ShapesManager.Shape.selectedShapeID + 1) + "/" + mysm.shapes.Count;
              mysm.lastSelectedGroup = ShapesManager.Shape.selectedShapeID;

                GameObject shapePrefab = mysm.GetCurrentShape().prefab;

                GameObject shapeGameObject = Instantiate(shapePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                shapeGameObject.transform.SetParent(shapeParent);
                shapeGameObject.transform.localPosition = shapePrefab.transform.localPosition;
                shapeGameObject.name = shapePrefab.name;

                compoundShape = GameObject.FindObjectOfType<CompoundShape>();

              
                    shape = GameObject.FindObjectOfType<Shape>();
                

                StartAutoTracing(shape, 0.5f);
                Invoke("Spell", 1f);
            }
            catch (System.Exception ex)
            {
                //Catch the exception or display an alert
                //Debug.LogError(ex.Message);
            }

            if (shape == null)
            {
                return;
            }

            //Set up shape's picture
            if (shapePicture != null)
            {
                shapePicture.sprite = mysm.GetCurrentShape().picture;
                if (shapePicture.sprite == null)
                {
                    shapePicture.enabled = false;
                }
                else
                {
                    shapePicture.enabled = true;
                }
            }

            //Set up write text/label
            if (writeText != null)
                writeText.text = "Write the " + mysm.shapeLabel.ToLower() + " '" + shape.GetTitle() + "'";

            //Setup rest message in the Rest Confirm Dialog
            CommonUtil.FindChildByTag(resetConfirmDialog.transform, "Message").GetComponent<Text>().text = "Reset " + mysm.shapeLabel + " " + shape.GetTitle() + " ?";

            EnableGameManager();
        }

        /// <summary>
        /// Go to the Next shape.
        /// </summary>
        public void NextShape()
        {
            if (ShapesManager.Shape.selectedShapeID >= 0 && ShapesManager.Shape.selectedShapeID < mysm.shapes.Count - 1)
            {
                //Get the next shape and check if it's locked , then do not load the next shape
                if (ShapesManager.Shape.selectedShapeID + 1 < mysm.shapes.Count)
                {

                    if (DataManager.IsShapeLocked(ShapesManager.Shape.selectedShapeID + 1, mysm) && !mysm.testMode)
                    {
                        //Play lock sound effectd
                        AudioSources.instance.PlayLockedSFX();
                        //Skip the next
                        return;
                    }
                }

                ShapesManager.Shape.selectedShapeID++;
                CreateShape();//Create new shape
            }
            else
            {
                if (ShapesManager.Shape.selectedShapeID == mysm.shapes.Count - 1)
                {
                   // UIEvents.instance.LoadAlbumScene();
                }
                else
                {
                    //Play lock sound effectd
                    AudioSources.instance.PlayLockedSFX();
                }
            }
        }

        /// <summary>
        /// Go to the previous shape.
        /// </summary>
        public void PreviousShape()
        {
            if (ShapesManager.Shape.selectedShapeID > 0 && ShapesManager.Shape.selectedShapeID < mysm.shapes.Count)
            {
                ShapesManager.Shape.selectedShapeID--;
                CreateShape();
            }
            else
            {
                //Play lock sound effectd
                AudioSources.instance.PlayLockedSFX();
            }
        }

        /// <summary>
        /// Trace the current path
        /// </summary>
        private void TracePath()
        {
            if (mysm.tracingMode == ShapesManager.TracingMode.FILL)
            {
                if (tracingPath.fillMethod == TracingPath.FillMethod.Radial)
                {
                    RadialFill();
                }
                else if (tracingPath.fillMethod == TracingPath.FillMethod.Linear)
                {
                    LinearFill();
                }
                else if (tracingPath.fillMethod == TracingPath.FillMethod.Point)
                {
                    PointFill();
                }
            }
            else if (mysm.tracingMode == ShapesManager.TracingMode.LINE)
            {
                DrawLine();
            }

            CheckPathComplete();
        }

        /// <summary>
        /// Radial fill tracing method.
        /// </summary>
        private void RadialFill()
        {
            clickPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            direction = clickPostion - tracingPath.curve.centroid;

            angleOffset = 0;
            clockWiseSign = (tracingPath.fillImage.fillClockwise ? 1 : -1);

            if (tracingPath.fillImage.fillMethod == Image.FillMethod.Radial360)
            {
                if (tracingPath.fillImage.fillOrigin == 0)
                {//Bottom
                    angleOffset = 0;
                }
                else if (tracingPath.fillImage.fillOrigin == 1)
                {//Right
                    angleOffset = clockWiseSign * 90;
                }
                else if (tracingPath.fillImage.fillOrigin == 2)
                {//Top
                    angleOffset = -180;
                }
                else if (tracingPath.fillImage.fillOrigin == 3)
                {//left
                    angleOffset = -clockWiseSign * 90;
                }
            }
            else if (tracingPath.fillImage.fillMethod == Image.FillMethod.Radial90)
            {
                if (tracingPath.fillImage.fillOrigin == 0)
                {//Bottom Left like path in 'a'
                    angleOffset = tracingPath.fillImage.fillClockwise ? -90 : 0;
                }
                else if (tracingPath.fillImage.fillOrigin == 3)
                {//Bottom Right like path in 'a' horinoal flipped
                    angleOffset = tracingPath.fillImage.fillClockwise ? 0 : -90;
                }
                else if (tracingPath.fillImage.fillOrigin == 2)
                {//Top Right like path in 'a' vertial flipped
                    angleOffset = tracingPath.fillImage.fillClockwise ? 90 : -180;
                }
                else if (tracingPath.fillImage.fillOrigin == 1)
                {//Top Left like path in 'a' vertial and horizontal flipped
                    angleOffset = tracingPath.fillImage.fillClockwise ? -180 : 90;
                }
            }

            angle = Mathf.Atan2(-clockWiseSign * direction.x, -direction.y) * Mathf.Rad2Deg + angleOffset;

            if (angle < 0)
                angle += 360;

            angle = Mathf.Clamp(angle, 0, 360);

            if (quarterRestriction)
            {
                if (!(angle >= 0 && angle <= targetQuarter))
                {
                    fillAmount = tracingPath.fillImage.fillAmount = 0;
                    return;
                }

                if (angle >= targetQuarter / 2)
                {
                    targetQuarter += 90;
                }
                else if (angle < 45)
                {
                    targetQuarter = 90;
                }

                targetQuarter = Mathf.Clamp(targetQuarter, 90, 360);
            }

            fillAmount = Mathf.Abs(angle / 360.0f);
            tracingPath.fillImage.fillAmount = fillAmount;
        }

        /// <summary>
        /// Linear fill tracing method.
        /// </summary>
        private void LinearFill()
        {
            clickPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Rect rect = CommonUtil.RectTransformToWorldSpace(tracingPath.GetComponent<RectTransform>());

            Vector3 pos1 = tracingPath.curve.GetFirstPoint().position, pos2 = tracingPath.curve.GetLastPoint().position;
            pos1.z = pos2.z = 0;

            clickPostion.x = Mathf.Clamp(clickPostion.x, Mathf.Min(pos1.x, pos2.x), Mathf.Max(pos1.x, pos2.x));
            clickPostion.y = Mathf.Clamp(clickPostion.y, Mathf.Min(pos1.y, pos2.y), Mathf.Max(pos1.y, pos2.y));
            clickPostion.z = 0;

            fillAmount = Vector2.Distance(clickPostion, pos1) / Vector2.Distance(pos1, pos2);
            tracingPath.fillImage.fillAmount = fillAmount;
        }

        /// <summary>
        /// Point fill tracing method.
        /// </summary>
        private void PointFill()
        {
            fillAmount = 1;
            tracingPath.fillImage.fillAmount = 1;
        }

        /// <summary>
        /// Draw line tracing method
        /// </summary>
        private void DrawLine()
        {
            //Get Line component
            if (tracingPath.line != null)
            {
                tracingPath.line.SetColor(CommonUtil.ColorToGradient(linecolor));
                //Add touch/click point into current line
                tracingPath.line.AddPoint(tracingPath.line.transform.InverseTransformPoint(GetCurrentPlatformClickPosition(Camera.main)));
                tracingPath.line.BezierInterpolate(0.3f);
            }
        }

        /// <summary>
        /// Checks wehther path completed or not.
        /// </summary>
        private void CheckPathComplete()
        {
            if (tracingPath.tracedPoints == tracingPath.curve.points.Count)
            {
                if (mysm.tracingMode == ShapesManager.TracingMode.FILL)
                { //Fill Tracing Mode
                    if (fillAmount >= tracingPath.completeOffset)
                    {
                        tracingPath.completed = true;
                        tracingPath.AutoFill();
                    }
                }
                else if (mysm.tracingMode == ShapesManager.TracingMode.LINE)
                {//Line Tracing Mode

                    if (Vector2.Distance(tracingPath.curve.GetLastPoint().position, GetCurrentPlatformClickPosition(Camera.main)) < 0.5f)
                    {
                        tracingPath.completed = true;
                    }
                }
            }

            if (tracingPath.completed)
            {
                tracingPath.OnComplete();
                ReleasePath();

                if (CheckShapeComplete())
                {
                    OnShapeComplete();
                }
                else
                {
                    AudioSources.instance.PlayCorrectSFX();
                }

                shape.ShowPathNumbers(shape.GetCurrentPathIndex());
            }
        }

        /// <summary>
        /// Check whether the shape completed or not.
        /// </summary>
        /// <returns><c>true</c>, if shape completed, <c>false</c> otherwise.</returns>
        private bool CheckShapeComplete()
        {
            bool shapeCompleted = true;
            foreach (TracingPath path in shape.tracingPaths)
            {
                if (!path.completed)
                {
                    shapeCompleted = false;
                    break;
                }
            }
            return shapeCompleted;
        }

        /// <summary>
        /// On shape completed event.
        /// </summary>
        private void OnShapeComplete()
        {
            shape.completed = true;

            bool allDone = true;

            List<Shape> shapes = new List<Shape>();

            if (compoundShape != null)
            {
                shapes = compoundShape.shapes;
                allDone = compoundShape.IsCompleted();

                if (!allDone)
                {
                    shape = compoundShape.shapes[compoundShape.GetCurrentShapeIndex()];
                    StartAutoTracing(shape, 1);
                }
            }
            else
            {
                shapes.Add(shape);
            }

            if (allDone)
            {
                SaveShapesData(shapes);

                DisableHandTracing();
                HideUserTouchHand();
                //brightEffect.gameObject.SetActive(false);

                foreach (Shape s in shapes)
                {
                    if (hideShapeImageOnComplete)
                        s.content.GetComponent<Image>().enabled = false;
                    s.animator.SetTrigger("Completed");
                }

                timer.Stop();
                BlackArea.instance.Show();
                winDialog.Show();
                nextButton.GetComponent<Animator>().SetTrigger("Select");
                winEffect.gameObject.SetActive(true);
                AudioSources.instance.PlayCompletedSFX();
                Invoke("NextShape", 2.5f);
              //  AdsManager.instance.HideAdvertisment();
              //  AdsManager.instance.ShowAdvertisment(AdPackage.AdEvent.Event.ON_SHOW_WIN_DIALOG, null);
            }
            else
            {
                AudioSources.instance.PlayCorrectSFX();
            }
        }

        /// <summary>
        /// Reset the shape.
        /// </summary>
        public void ResetShape()
        {
            List<Shape> shapes = new List<Shape>();
            if (compoundShape != null)
            {
                shapes = compoundShape.shapes;
            }
            else
            {
                shapes.Add(shape);
            }

            winEffect.gameObject.SetActive(false);
            nextButton.GetComponent<Animator>().SetBool("Select", false);
            BlackArea.instance.Hide();
            winDialog.Hide();

            DisableHandTracing();

            foreach (Shape s in shapes)
            {
                if (s == null)
                    continue;

                s.completed = false;
                s.content.GetComponent<Image>().enabled = true;
                s.animator.SetBool("Completed", false);
                s.CancelInvoke();
                TracingPath[] paths = s.GetComponentsInChildren<TracingPath>();
                foreach (TracingPath path in paths)
                {
                    path.Reset();
                }

                if (compoundShape == null)
                {
                    StartAutoTracing(s, 2);
                }
                else if (compoundShape.GetShapeIndexByInstanceID(s.GetInstanceID()) == 0)
                {
                    shape = compoundShape.shapes[0];
                    StartAutoTracing(shape, 2);
                }
                AudioSources.instance.PlaySFXClip(mysm.GetCurrentShape().clip, false);
            }

            ReleasePath();

            Spell();
            timer.Reset();
        }

        /// <summary>
        /// Save the data of the shapes such as (stars,path colors,unlock next shape...) .
        /// </summary>
        private void SaveShapesData(List<Shape> shapes)
        {
            if (shapes == null)
            {
                return;
            }

            if (shapes.Count == 0)
            {
                return;
            }

            //Save collected stars , stars of the shape
            ShapesManager.Shape.StarsNumber collectedStars = Progress.instance.starsNumber;
            DataManager.SaveShapeStars(ShapesManager.Shape.selectedShapeID, collectedStars, mysm);

            int collectedStarsOffset = CommonUtil.ShapeStarsNumberEnumToIntNumber(collectedStars) - CommonUtil.ShapeStarsNumberEnumToIntNumber(mysm.GetCurrentShape().starsNumber);
            mysm.SetCollectedStars(collectedStarsOffset + mysm.GetCollectedStars());

            mysm.GetCurrentShape().starsNumber = collectedStars;

            //unlock the next shape
            if (ShapesManager.Shape.selectedShapeID + 1 < mysm.shapes.Count)
            {
                DataManager.SaveShapeLockedStatus(ShapesManager.Shape.selectedShapeID + 1, false, mysm);
                mysm.shapes[ShapesManager.Shape.selectedShapeID + 1].isLocked = false;
            }

            Color tempColor = Colors.whiteColor;
            // save the colors of the paths
            int compundID = -1;
            foreach (Shape s in shapes)
            {
                if (compoundShape != null)
                {
                    //ID or Index of the shape in the compound shape
                    compundID = compoundShape.GetShapeIndexByInstanceID(s.GetInstanceID());
                }

                foreach (TracingPath p in s.tracingPaths)
                {
                    tempColor = mysm.tracingMode == ShapesManager.TracingMode.FILL ? linecolor : linecolor;
                    DataManager.SaveShapePathColor(ShapesManager.Shape.selectedShapeID, compundID, p.from, p.to, tempColor, mysm);
                }
            }
        }

        /// <summary>
        /// Starts the auto tracing for the current path.
        /// </summary>
        /// <param name="s">Shape Reference.</param>
        public void StartAutoTracing(Shape s, float traceDelay)
        {
            if (s == null)
            {
                return;
            }

            //Stop current movement
            DisableHandTracing();

            int currentPathIndex = s.GetCurrentPathIndex();

            if (currentPathIndex < 0 || currentPathIndex > s.tracingPaths.Count - 1)
                return;

            //Hide Numbers for other shapes , if we have compound shape
            if (compoundShape != null)
            {
                foreach (Shape ts in compoundShape.shapes)
                {
                    if (s.GetInstanceID() != ts.GetInstanceID())
                        ts.ShowPathNumbers(-1);
                }
            }

            if (s.tracingPaths.Count != 0)
            {
                //Set up the curve , and set position of Hand to the first point
                handPOM.curve = s.tracingPaths[currentPathIndex].curve;
                handPOM.transform.position = s.tracingPaths[currentPathIndex].curve.points[0].transform.position;
            }

            s.ShowPathNumbers(currentPathIndex);

            //Move the hand
            Invoke("EnableHandTracing", traceDelay);
        }

        /// <summary>
        /// Spell the shape.
        /// </summary>
        public void Spell()
        {
            if (mysm.GetCurrentShape().clip == null)
            {
                return;
            }

            AudioSources.instance.PlaySFXClip(mysm.GetCurrentShape().clip, false);
        }

        /// <summary>
        /// Help the user.
        /// </summary>
        public void HelpUser()
        {
            int currentPathIndex = shape.GetCurrentPathIndex();

            if (currentPathIndex < 0 || currentPathIndex > shape.tracingPaths.Count - 1)
            {
                return;
            }

            tracingPath = shape.tracingPaths[currentPathIndex];

            if (mysm.tracingMode == ShapesManager.TracingMode.FILL)
            {
                tracingPath.fillImage.color = currentPencil.color.colorKeys[0].color;
                tracingPath.AutoFill();
            }
            else if (mysm.tracingMode == ShapesManager.TracingMode.LINE)
            {
                tracingPath.line.SetColor(CommonUtil.ColorToGradient(currentPencil.color.colorKeys[0].color));
            }

            tracingPath.completed = true;
            tracingPath.OnComplete();

            if (CheckShapeComplete())
            {
                OnShapeComplete();
            }
            else
            {
                AudioSources.instance.PlayCorrectSFX();
            }
            tracingPath = null;
        }

        /// <summary>
        /// Reset the path.
        /// </summary>
        private void ResetPath()
        {
            if (tracingPath != null)
                tracingPath.Reset();
            ReleasePath();
        }

        /// <summary>
        /// Release the path.
        /// </summary>
        private void ReleasePath()
        {
            tracingPath = null;
            fillAmount = angleOffset = angle = 0;
            ResetTargetQuarter();
        }

        /// <summary>
        /// Reset the target quarter.
        /// </summary>
        private void ResetTargetQuarter()
        {
            targetQuarter = 90;
        }

        /// <summary>
        /// Enable the auto tracing of the hand.
        /// </summary>
        public void EnableHandTracing()
        {
            if (tracingPath != null)
            {
                handPOM.curve = tracingPath.curve;
            }

            if (handPOM.curve != null)
                handPOM.curve.Init();
            handPOM.MoveToStart();
        }

        /// <summary>
        /// Disable the auto tracing of the hand.
        /// </summary>
        public void DisableHandTracing()
        {
            CancelInvoke("EnableHandTracing");
            handPOM.Stop();
        }

        /// <summary>
        /// Show User's click/touch hand.
        /// </summary>
        public void ShowUserTouchHand()
        {
            hand.GetComponent<SpriteRenderer>().enabled = true;
        }

        /// <summary>
        /// Hide User's click/touch hand.
        /// </summary>
        public void HideUserTouchHand()
        {
            hand.GetComponent<SpriteRenderer>().enabled = false;
        }

        /// <summary>
        /// Draw the hand.
        /// </summary>
        /// <param name="clickPosition">Click position.</param>
        private void DrawHand(Vector3 clickPosition)
        {
            if (hand == null)
            {
                return;
            }

            hand.transform.position = clickPosition;
        }

        /// <summary>
        /// Set the size of the hand to default size.
        /// </summary>
        private void SetHandDefaultSize()
        {
            hand.transform.localScale = cursorDefaultSize;
        }

        /// <summary>
        /// Set the size of the hand to click size.
        /// </summary>
        private void SetHandClickSize()
        {
            hand.transform.localScale = cursorClickSize;
        }

        /// <summary>
        /// Draw the bright effect.
        /// </summary>
        /// <param name="clickPosition">Click position.</param>
        private void DrawBrightEffect(Vector3 clickPosition)
        {
            /*
            if (brightEffect == null) {
                return;
            }

            clickPosition.z = 0;
            brightEffect.transform.position = clickPosition;
            */
        }

        /// <summary>
        /// Set the color of the shape order.
        /// </summary>
        public void SetShapeOrderColor()
        {
            if (currentPencil == null)
            {
                return;
            }
           // shapeOrder.color = currentPencil.color.colorKeys[0].color;
        }

        /// <summary>
        /// Disable the game manager.
        /// </summary>
        public void DisableGameManager()
        {
            isRunning = false;
        }

        /// <summary>
        /// Enable the game manager.
        /// </summary>
        public void EnableGameManager()
        {
            isRunning = true;
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        public void Pause()
        {
            if (!isRunning)
            {
                return;
            }

            if (Timer.instance != null)
                Timer.instance.Pause();
            DisableGameManager();
        }

        /// <summary>
        /// Pause the gamse.
        /// </summary>
        public void Resume()
        {
            if (Timer.instance != null)
                Timer.instance.Resume();
            EnableGameManager();
        }
    }
}