using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

namespace IndieStudio.EnglishTracingBook.Utility
{
    [DisallowMultipleComponent]
    public class SingletonManager : MonoBehaviour
    {

        public GameObject[] values;

        // Use this for initialization
        void Awake()
        {
            InitManagers();
        }

        private void InitManagers()
        {
            if (values == null)
            {
                return;
            }

            foreach (GameObject value in values)
            {
                if (GameObject.Find(value.name) == null)
                {
                    GameObject temp = Instantiate(value, Vector3.zero, Quaternion.identity) as GameObject;
                    temp.name = value.name;
                }
            }
        }

        /// <summary>
        /// Returns all Shapes Manager instances from values
        /// </summary>
        /// <param name="excluded">Names of excluded shapes manager</param>
        /// <returns></returns>
        public List<ShapesManager> GetShapesManagers(string[] excluded)
        {
            List<ShapesManager> shapesManagers = new List<ShapesManager>();

            bool skip;
            foreach (GameObject value in values)
            {
                skip = false;

                ShapesManager sm = value.GetComponent<ShapesManager>();
                if (sm != null)
                {
                    if (excluded != null)
                    {
                        foreach (string s in excluded)
                        {
                            if (s == sm.name)
                            {
                                skip = true;
                                break;
                            }
                        }
                    }

                    if (!skip)
                        shapesManagers.Add(sm);
                }
            }
            return shapesManagers;
        }
    }
}