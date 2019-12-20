using UnityEngine;
using UnityEditor;

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

public class PostProcessor : AssetPostprocessor
{
	private static readonly string googleMobileAdsPath = Application.dataPath + "/GoogleMobileAds";
	private static readonly  string chartBoostAdsPath = Application.dataPath + "/Chartboost";
	private static readonly  string unityAdsPath = Application.dataPath + "/UnityAds";
	private static readonly string googleMobileAdsDefine = "GOOGLE_MOBILE_ADS;";
	private static readonly string chartBoosteAdsDefine = "CHARTBOOST_ADS;";
	private static readonly string unityAdsDefine ="UNITY_ADS;";

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		string defines = "";

		if (System.IO.Directory.Exists (googleMobileAdsPath)) {
			defines += googleMobileAdsDefine;
		}

		if (System.IO.Directory.Exists (chartBoostAdsPath)) {
			defines += chartBoosteAdsDefine;
		}

		if (System.IO.Directory.Exists (unityAdsPath)) {
			defines += unityAdsDefine;
		}

		if (!string.IsNullOrEmpty (defines)) {
			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.Android, defines);
			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.iOS, defines);
		}
	}
}