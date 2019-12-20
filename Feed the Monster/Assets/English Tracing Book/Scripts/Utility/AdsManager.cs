using UnityEngine;
using UnityEngine.Events;


#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
using System;
using System.Collections.Generic;

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
    [ExecuteInEditMode]
    [RequireComponent(typeof(AdMob))]
    [RequireComponent(typeof(ChartboostAd))]
    [DisallowMultipleComponent]
    public class AdsManager : MonoBehaviour
    {
        /// <summary>
        /// The admob reference.
        /// </summary>
        private AdMob admob;

        /// <summary>
        /// The chart boost ad reference.
        /// </summary>
        private ChartboostAd chartBoostAd;

        /// <summary>
        /// The unity ad reference.
        /// </summary>
        private UnityAd unityAd;

        /// <summary>
        /// This Gameobject defined as a Singleton.
        /// </summary>
        public static AdsManager instance;

        /// <summary>
        /// A list of AdPackage.
        /// </summary>
        public List<AdPackage> adPackages = new List<AdPackage>();

        void Awake()
        {
            if (Application.isPlaying)
            {
                Init();
            }
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                Build();
            }
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        private void Init()
        {
            if (instance == null)
            {
                instance = this;
               // DontDestroyOnLoad(gameObject);
                if (admob == null)
                    admob = GetComponent<AdMob>();
                if (chartBoostAd == null)
                    chartBoostAd = GetComponent<ChartboostAd>();
                if (unityAd == null)
                    unityAd = GetComponent<UnityAd>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Build AdPackages & AdEvent lists.
        /// </summary>
        public void Build()
        {
            BuildAdPackages();
            foreach (AdPackage adPackage in adPackages)
            {
                adPackage.BuildAdEvents();
            }
        }

        /// <summary>
        /// Show the advertisment.
        /// </summary>
        /// <param name="evt">Event.</param>
        public void ShowAdvertisment(AdPackage.AdEvent.Event evt, UnityEvent onShowAdsEvent)
        {
            if (adPackages == null)
            {
                return;
            }

            bool eventFound = false;
            foreach (AdPackage adPackage in adPackages)
            {

                if (!adPackage.isEnabled)
                {
                    continue;
                }

                if (adPackage.adEvents == null)
                {
                    return;
                }

                if (eventFound)
                {
                    //remove the comment below to allow single advertisment per event between all APIS
                    //break;
                }

                foreach (AdPackage.AdEvent adEvent in adPackage.adEvents)
                {
                    if (adEvent.evt == evt)
                    {
                        if (adEvent.isEnabled)
                        {
                            if (adPackage.package == AdPackage.Package.ADMOB)
                            {
                                AdMobAdvertisment(adEvent, onShowAdsEvent);
                            }
                            else if (adPackage.package == AdPackage.Package.CHARTBOOST)
                            {
                                ChartBoostAdvertisment(adEvent, onShowAdsEvent);
                            }
                            else if (adPackage.package == AdPackage.Package.UNITY)
                            {
                                UnityAdvertisment(onShowAdsEvent);
                            }
                            eventFound = true;
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Hide the advertisment.
        /// </summary>
        public void HideAdvertisment()
        {
            foreach (AdPackage adPackage in adPackages)
            {
                if (!adPackage.isEnabled)
                {
                    continue;
                }
                if (adPackage.package == AdPackage.Package.ADMOB)
                {
#if GOOGLE_MOBILE_ADS
				if (string.IsNullOrEmpty (admob.androidBannerAdUnitID) && string.IsNullOrEmpty (admob.IOSBannerAdUnitID)) {
					Debug.LogWarning ("Banner AdUnit is not defined in AdMob component");
					return;
				}

				admob.DestroyBannerAd ();
#endif
                }
            }
        }

        /// <summary>
        /// Show the Admob advertisment.
        /// </summary>
        /// <param name="adEvent">Ad event.</param>
        private void AdMobAdvertisment(AdPackage.AdEvent adEvent, UnityEvent onShowAdsEvent)
        {
#if GOOGLE_MOBILE_ADS
		if (adEvent.type == AdPackage.AdEvent.Type.BANNER) {
			//Request and show banner advertisment
			if (string.IsNullOrEmpty (admob.androidBannerAdUnitID) && string.IsNullOrEmpty (admob.IOSBannerAdUnitID)) {
				Debug.LogWarning ("Banner AdUnit is not defined in AdMob component");
				return;
			}

			admob.RequestBannerAd (adEvent.adPostion);
		} else if (adEvent.type == AdPackage.AdEvent.Type.INTERSTITIAL) {
			//Show Interstitial Advertisment
			if (string.IsNullOrEmpty (admob.androidInterstitialAdUnitID) && string.IsNullOrEmpty (admob.IOSInterstitialAdUnitID)) {
				Debug.LogWarning ("Interstitial AdUnit is not defined in AdMob component");
				return;
			}
			admob.ShowInterstitialAd (onShowAdsEvent);
		} else if (adEvent.type == AdPackage.AdEvent.Type.RewardBasedVideo) {
			if (string.IsNullOrEmpty (admob.androidRewardBasedVideoAdUnitID) && string.IsNullOrEmpty (admob.IOSRewardBasedVideoAdUnitID)) {
				Debug.LogWarning ("RewardBasedVideo AdUnit is not defined in AdMob component");
				return;
			}
			//Show RewardBasedVideo Advertisment
			admob.ShowRewardBasedVideoAd (onShowAdsEvent);
		}
#endif
        }

        /// <summary>
        /// Show the ChartBoost advertisment.
        /// </summary>
        /// <param name="adEvent">Ad event.</param>
        private void ChartBoostAdvertisment(AdPackage.AdEvent adEvent, UnityEvent onShowAdsEvent)
        {
#if CHARTBOOST_ADS
			if (adEvent.type == AdPackage.AdEvent.Type.INTERSTITIAL) {
				//Show chartboost Interstitial Advertisment
				chartBoostAd.ShowInterstitial (onShowAdsEvent);
			} else if (adEvent.type == AdPackage.AdEvent.Type.RewardBasedVideo) {
				//Show chartboost RewardBasedVideo Advertisment
				chartBoostAd.ShowRewardedVideo (onShowAdsEvent);
			}
#endif
        }

        /// <summary>
        /// Show the Unity advertisment.
        /// </summary>
        private void UnityAdvertisment(UnityEvent onShowAdsEvent)
        {
            unityAd.ShowUnityAd(onShowAdsEvent);
        }

        /// <summary>
        /// Build the ad Package.
        /// </summary>
        public void BuildAdPackages()
        {
            Array adPackageEnum = Enum.GetValues(typeof(AdPackage.Package));

            if (NeedsToResetPackagesList(adPackageEnum, adPackages))
            {
                adPackages.Clear();
            }

            foreach (AdPackage.Package p in adPackageEnum)
            {
                if (!InAdPackagesList(adPackages, p))
                {
                    adPackages.Add(new AdPackage() { package = p });
                }
            }
        }

        /// <summary>
        /// Whether the package in the adPackagees list
        /// </summary>
        /// <returns><c>true</c>, if package was found, <c>false</c> otherwise.</returns>
        /// <param name="adPackagees">Ad Packages List.</param>
        /// <param name="package">The given package.</param>
        public bool InAdPackagesList(List<AdPackage> adPackagees, AdPackage.Package package)
        {
            if (adPackages == null)
            {
                return false;
            }

            foreach (AdPackage adPackage in adPackages)
            {
                if (adPackage.package == package)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Whether to reset Packages list or not.
        /// </summary>
        /// <returns><c>true</c>, if reset Packages list was needed, <c>false</c> otherwise.</returns>
        /// <param name="adPackageEnum">Ad Package enum.</param>
        /// <param name="adPackages">Ad Packages.</param>
        public bool NeedsToResetPackagesList(Array adPackageEnum, List<AdPackage> adPackages)
        {
            if (adPackageEnum.Length != adPackages.Count)
            {
                return true;
            }

            return false;
        }
    }
}