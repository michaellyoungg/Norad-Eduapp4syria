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
    public class Pointer : MonoBehaviour
    {
        public Group group;//the group reference

        /// <summary>
        /// Create a pointer.
        /// </summary>
        /// <param name="groupIndex">Group index.</param>
        /// <param name="levelsGroup">Levels group.</param>
        /// <param name="pointerPrefab">Pointer prefab.</param>
        /// <param name="pointersParent">Pointers parent.</param>
        public static void CreatePointer(int groupIndex, GameObject levelsGroup, GameObject pointerPrefab, Transform pointersParent)
        {
            if (levelsGroup == null || pointerPrefab == null || pointersParent == null)
            {
                return;
            }

            //Create Slider Pointer
            GameObject pointer = Instantiate(pointerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            pointer.transform.SetParent(pointersParent);
            pointer.name = "Pointer-" + CommonUtil.IntToString(groupIndex + 1);
            pointer.transform.localScale = Vector3.one;
            pointer.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            pointer.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            pointer.GetComponent<Pointer>().group = levelsGroup.GetComponent<Group>();
            pointer.GetComponent<Button>().onClick.AddListener(() => UIEvents.instance.PointerButtonEvent(pointer.GetComponent<Pointer>()));
        }
    }
}