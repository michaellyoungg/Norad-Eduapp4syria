using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;
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

public class MenusEditor : Editor
{
    [MenuItem("Tools/English Tracing Book/Game Scene/Create Sentence OR Word #s", false, 0)]
    static void CreateSentence()
    {
        GameObject newSentece = new GameObject("new-sentence");
        RectTransform rectTransform = newSentece.AddComponent<RectTransform>();
        newSentece.AddComponent<CompoundShape>();
        newSentece.transform.SetParent(GameObject.Find("Shape").transform);
        newSentece.transform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector3.zero;
        rectTransform.anchorMax = Vector3.one;
        rectTransform.offsetMax = rectTransform.offsetMin = Vector3.zero;
        newSentece.transform.localPosition = Vector3.zero;
        Selection.activeObject = newSentece;
    }

    [MenuItem("Tools/English Tracing Book/Game Scene/Create Sentence OR Word #s", true, 0)]
    static bool CreateSentenceValidate()
    {
        return !Application.isPlaying && SceneManager.GetActiveScene().name == "Game";
    }

    [MenuItem("Tools/English Tracing Book/Delete PlayerPrefs", false, 10)]
    static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
