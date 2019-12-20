using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public class StarsRate : MonoBehaviour
    {

        /// <summary>
        /// The references of the stars images.
        /// </summary>
        public Image[] stars;

        /// <summary>
        /// The star on,off sprites.
        /// </summary>
        public Sprite starOn, starOff;

        /// <summary>
        /// The shapes manager reference as name.
        /// </summary>
        public string shapesManagerReference;

        void Start()
        {

            //Setting up the stars rate
            ShapesManager shapesManager = ShapesManager.shapesManagers[shapesManagerReference];
            int starsRate = shapesManager.GetStarsRate();

            if (starsRate == 0)
            {//Zero Stars
                stars[0].sprite = starOff;
                stars[1].sprite = starOff;
                stars[2].sprite = starOff;
            }
            else if (starsRate == 1)
            {//One Star
                stars[0].sprite = starOn;
                stars[1].sprite = starOff;
                stars[2].sprite = starOff;
            }
            else if (starsRate == 2)
            {//Two Stars
                stars[0].sprite = starOn;
                stars[1].sprite = starOn;
                stars[2].sprite = starOff;
            }
            else
            {//Three Stars
                stars[0].sprite = starOn;
                stars[1].sprite = starOn;
                stars[2].sprite = starOn;
            }
        }
    }
}