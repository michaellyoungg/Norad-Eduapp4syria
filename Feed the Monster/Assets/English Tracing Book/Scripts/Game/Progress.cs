using UnityEngine;
using System.Collections;
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
    [DisallowMultipleComponent]
    public class Progress : MonoBehaviour
    {
        /// <summary>
        /// The star off sprite.
        /// </summary>
        public Sprite starOff;

        /// <summary>
        /// The star on sprite.
        /// </summary>
        public Sprite starOn;

        /// <summary>
        /// Droped star flags.
        /// </summary>
        private bool[] dropedStar;

        /// <summary>
        /// The level stars.
        /// </summary>
        public Image[] levelStars;

        /// <summary>
        /// The progress image.
        /// </summary>
        public Image progressImage;

        /// <summary>
        /// The stars number.
        /// </summary>
        [HideInInspector]
        public ShapesManager.Shape.StarsNumber starsNumber;

        /// <summary>
        /// Top Canvas transform
        /// </summary>
        private Transform topCanvas;

        /// <summary>
        /// Static instance of this class.
        /// </summary>
        public static Progress instance;

        void Awake()
        {
            if (instance == null)
                instance = this;

            if (topCanvas == null)
            {
                topCanvas = GameObject.Find("TopCanvas").transform;
            }

            if (progressImage == null)
            {
                progressImage = GetComponent<Image>();
            }
            dropedStar = new bool[3];
        }

        // Use this for initialization
        void Start()
        {
         
        }

        /// <summary>
        /// Set the value of the progress.
        /// </summary>
        /// <param name="currentTime">Current time.</param>
        public void SetProgress(float currentTime)
        {
            if (GameManager.instance == null)
            {
                return;
            }

            if (GameManager.instance.shape == null)
            {
                return;
            }

            if (progressImage != null)
                progressImage.fillAmount = 1 - (currentTime / (ShapesManager.GetCurrentShapesManager().GetCurrentShape().starsTimePeriod * 3));

            if (currentTime >= 0 && currentTime <= ShapesManager.GetCurrentShapesManager().GetCurrentShape().starsTimePeriod)
            {
                if (levelStars[0] != null)
                {
                    levelStars[0].sprite = starOn;
                }
                if (levelStars[1] != null)
                {
                    levelStars[1].sprite = starOn;
                }
                if (levelStars[2] != null)
                {
                    levelStars[2].sprite = starOn;
                }
                if (progressImage != null)
                    progressImage.color = Colors.greenColor;

                starsNumber = ShapesManager.Shape.StarsNumber.THREE;
            }
            else if (currentTime > ShapesManager.GetCurrentShapesManager().GetCurrentShape().starsTimePeriod && currentTime <= 2 * ShapesManager.GetCurrentShapesManager().GetCurrentShape().starsTimePeriod)
            {
                if (levelStars[2] != null)
                {
                    levelStars[2].sprite = starOff;
                }
                if (progressImage != null)
                    progressImage.color = Colors.yellowColor;
                starsNumber = ShapesManager.Shape.StarsNumber.TWO;
                DropThirdStar();
            }
            else
            {
                if (levelStars[1] != null)
                {
                    levelStars[1].sprite = starOff;
                }
                if (levelStars[2] != null)
                {
                    levelStars[2].sprite = starOff;
                }
                if (progressImage != null)
                    progressImage.color = Colors.redColor;
                starsNumber = ShapesManager.Shape.StarsNumber.ONE;
                DropSecondStar();
            }
        }

        /// <summary>
        /// Drop the first star.
        /// </summary>
        private void DropFirstStar()
        {
            if (dropedStar[0])
            {
                return;
            }

            dropedStar[0] = true;
            DropStarEffect(levelStars[0].transform);
        }

        /// <summary>
        /// Drop the second star.
        /// </summary>
        private void DropSecondStar()
        {
            if (dropedStar[1])
            {
                return;
            }

            dropedStar[1] = true;
            DropStarEffect(levelStars[1].transform);
        }

        /// <summary>
        /// Drop the third star.
        /// </summary>
        private void DropThirdStar()
        {
            if (dropedStar[2])
            {
                return;
            }

            dropedStar[2] = true;
            DropStarEffect(levelStars[2].transform);
        }

        /// <summary>
        /// Reset drop flags
        /// </summary>
        public void ResetDropFlags()
        {
            for (int i = 0; i < dropedStar.Length; i++)
            {
                dropedStar[i] = false;
            }
        }

        /// <summary>
        /// Drop star effect.
        /// </summary>
        /// <param name="star">Star transform reference.</param>
        private void DropStarEffect(Transform star)
        {
            if (star == null)
            {
                return;
            }

            RectTransform startRect = star.GetComponent<RectTransform>();

            GameObject tempInstance = Instantiate(star.gameObject, star.position, Quaternion.identity) as GameObject;
            tempInstance.transform.SetParent(star.parent);
            tempInstance.transform.localScale = star.localScale;
            RectTransform tempInstanceRectTransform = tempInstance.GetComponent<RectTransform>();
            tempInstanceRectTransform.offsetMax = startRect.offsetMax;
            tempInstanceRectTransform.offsetMin = startRect.offsetMin;
            tempInstance.transform.SetParent(topCanvas);
            tempInstanceRectTransform.SetSiblingIndex(GameManager.instance.resetConfirmDialog.GetComponent<RectTransform>().GetSiblingIndex() - 1);

            tempInstance.GetComponent<Image>().sprite = starOn;
            tempInstance.GetComponent<Rotate>().Enabled = false;

            Rigidbody2D tempInstanceRB = tempInstance.GetComponent<Rigidbody2D>();
            tempInstanceRB.isKinematic = false;
            tempInstanceRB.AddForce(new Vector2(0.5f, 0.2f) * 300);
            tempInstanceRB.AddTorque(360);

            tempInstance.GetComponentInChildren<TrailRenderer>().enabled = true;

            AudioSources.instance.PlayDropStarSFX();

            Destroy(tempInstance, 3);
        }
    }
}