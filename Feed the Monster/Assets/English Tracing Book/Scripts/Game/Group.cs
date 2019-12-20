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
    public class Group : MonoBehaviour
    {
        public int Index;//the group's Index

        /// <summary>
        /// Create a group.
        /// </summary>
        /// <returns>The group.</returns>
        /// <param name="levelsGroupPrefab">Levels group prefab.</param>
        /// <param name="groupsParent">Groups parent.</param>
        /// <param name="groupIndex">Group index.</param>
        /// <param name="columnsPerGroup">Columns per group.</param>
        public static GameObject CreateGroup(GameObject levelsGroupPrefab, Transform groupsParent, int groupIndex, int columnsPerGroup)
        {
            //Create Levels Group
            GameObject levelsGroup = Instantiate(levelsGroupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            levelsGroup.transform.SetParent(groupsParent);
            levelsGroup.name = "Group-" + CommonUtil.IntToString(groupIndex + 1);
            levelsGroup.transform.localPosition = Vector3.zero;
            levelsGroup.transform.localScale = Vector3.one;
            levelsGroup.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            levelsGroup.GetComponent<RectTransform>().offsetMin = Vector2.zero;
            levelsGroup.GetComponent<Group>().Index = groupIndex;
            levelsGroup.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            levelsGroup.GetComponent<GridLayoutGroup>().constraintCount = columnsPerGroup;
            return levelsGroup;
        }
    }
}