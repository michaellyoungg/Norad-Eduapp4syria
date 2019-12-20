using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;


#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif


namespace IndieStudio.EnglishTracingBook.Utility
{
    [DisallowMultipleComponent]
    public class AdMob : MonoBehaviour
    {
#if GOOGLE_MOBILE_ADS
		private BannerView bannerView;
		private InterstitialAd interstitialAd;
		private RewardBasedVideoAd rewardBasedVideoAd;
		public string androidAppId;
		public string IOSAppId;
		public string androidBannerAdUnitID;
		public string IOSBannerAdUnitID;
		public string androidInterstitialAdUnitID;
		public string IOSInterstitialAdUnitID;
		public string androidRewardBasedVideoAdUnitID;
		public string IOSRewardBasedVideoAdUnitID;

		void Start ()
		{
#if UNITY_ANDROID
					string appId = androidAppId;
#elif UNITY_IPHONE
					string appId = IOSAppId;
#else
					string appId = "unexpected_platform";
#endif

				appId = appId.Trim ();

				if (string.IsNullOrEmpty (appId)) {
					Debug.LogWarning ("App ID is undefined");
				}

				// Initialize the Google Mobile Ads SDK.
				MobileAds.Initialize(appId);

				// Get singleton reward based video ad reference.
				this.rewardBasedVideoAd = RewardBasedVideoAd.Instance;

				this.rewardBasedVideoAd.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
				this.rewardBasedVideoAd.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
				this.rewardBasedVideoAd.OnAdOpening += this.HandleRewardBasedVideoOpened;
				this.rewardBasedVideoAd.OnAdStarted += this.HandleRewardBasedVideoStarted;
				this.rewardBasedVideoAd.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
				this.rewardBasedVideoAd.OnAdClosed += this.HandleRewardBasedVideoClosed;
				this.rewardBasedVideoAd.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;

				RequestInterstitialAd();
				RequestRewardBasedVideoAd ();
		}

		public void RequestBannerAd (AdPosition adPostion)
		{
				if (adPostion == null) {
						return;
				}

#if UNITY_ANDROID
					string adUnitId = androidBannerAdUnitID;
#elif UNITY_IPHONE
					string adUnitId = IOSBannerAdUnitID;
#else
					string adUnitId = "unexpected_platform";
#endif

				adUnitId = adUnitId.Trim();

				if (string.IsNullOrEmpty (adUnitId)) {
					return;
				}

				//Destroy current banner ad, if exists
				DestroyBannerAd ();

				// Create a banner
				this.bannerView = new BannerView (adUnitId, AdSize.Banner, adPostion);

				// Register for ad events.
				this.bannerView.OnAdLoaded += this.HandleBannerLoaded;
				this.bannerView.OnAdFailedToLoad += this.HandleBannerFailedToLoad;
				this.bannerView.OnAdOpening += this.HandleBannerOpened;
				this.bannerView.OnAdClosed += this.HandleBannerClosed;
				this.bannerView.OnAdLeavingApplication += this.HandleBannerLeftApplication;

				// Create an empty ad request.
				AdRequest request = new AdRequest.Builder ().Build ();

				// Load the banner with the request.
				this.bannerView.LoadAd (request);
		}

		private void RequestInterstitialAd ()
		{
#if UNITY_ANDROID
					string adUnitId = androidInterstitialAdUnitID;
#elif UNITY_IPHONE
					string adUnitId = IOSInterstitialAdUnitID;
#else
					string adUnitId = "unexpected_platform";
#endif

				adUnitId = adUnitId.Trim();
				
				if (string.IsNullOrEmpty (adUnitId)) {
					return;
				}

				//Destroy current Interstitial ad, if exists
				DestroyInterstitialAd ();
	
				// Initialize an InterstitialAd.
				this.interstitialAd = new InterstitialAd (adUnitId);

				// Register for ad events.
				this.interstitialAd.OnAdLoaded += this.HandleInterstitialLoaded;
				this.interstitialAd.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
				this.interstitialAd.OnAdOpening += this.HandleInterstitialOpened;
				this.interstitialAd.OnAdClosed += this.HandleInterstitialClosed;
				this.interstitialAd.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

				// Create an empty ad request.
				AdRequest request = new AdRequest.Builder ().Build ();

				// Load the interstitial with the request.
				this.interstitialAd.LoadAd (request);
		}

		private void RequestRewardBasedVideoAd ()
		{
#if UNITY_ANDROID
					string adUnitId = androidRewardBasedVideoAdUnitID;
#elif UNITY_IPHONE
					string adUnitId = IOSRewardBasedVideoAdUnitID;
#else
					string adUnitId = "unexpected_platform";
#endif

				adUnitId = adUnitId.Trim();

				if (string.IsNullOrEmpty (adUnitId)) {
					return;
				}

				// Create an empty ad request.
				AdRequest request = new AdRequest.Builder ().Build ();

				this.rewardBasedVideoAd.LoadAd (request, adUnitId);
		}

		// Returns an ad request with custom ad targeting.
		private AdRequest CreateAdRequest()
		{
			return new AdRequest.Builder()
			.AddTestDevice(AdRequest.TestDeviceSimulator)
			.AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
			.AddKeyword("game")
			.SetGender(Gender.Male)
			.SetBirthday(new DateTime(1985, 1, 1))
			.TagForChildDirectedTreatment(false)
			.AddExtra("color_bg", "9B30FF")
			.Build();
		}

		private void ShowBannerAd ()
		{
				if (this.bannerView == null) {
						return;
				}
				this.bannerView.Show ();
		}

		public void ShowInterstitialAd (UnityEvent onShowAdsEvent)
		{
				if (this.interstitialAd == null) {
						return;
				}

				if (this.interstitialAd.IsLoaded ()) {
						if(onShowAdsEvent!=null)
							onShowAdsEvent.Invoke ();
						this.interstitialAd.Show ();
				}
		}

		public void ShowRewardBasedVideoAd (UnityEvent onShowAdsEvent)
		{
				if (this.rewardBasedVideoAd == null) {
						return;
				}

				if (rewardBasedVideoAd.IsLoaded ()) {
					if(onShowAdsEvent!=null)
						onShowAdsEvent.Invoke ();
					this.rewardBasedVideoAd.Show ();
				}
		}

		public void DestroyBannerAd ()
		{
				if (this.bannerView == null) {
						return;
				}
				this.bannerView.Destroy ();
		}

		private void DestroyInterstitialAd ()
		{
				if (this.interstitialAd == null) {
						return;
				}
				this.interstitialAd.Destroy ();
		}

        #region Banner callback handlers

		private void HandleBannerLoaded (object sender, EventArgs args)
		{
				ShowBannerAd ();
				MonoBehaviour.print ("HandleBannerLoaded event received");
		}

		private void HandleBannerFailedToLoad (object sender, AdFailedToLoadEventArgs args)
		{
				//(Optional)Try to request new one
				MonoBehaviour.print ("HandleBannerFailedToLoad event received with message: " + args.Message);
		}

		private void HandleBannerOpened (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleBannerOpened event received");
		}

		private void HandleBannerClosed (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleBannerClosed event received");
		}

		private void HandleBannerLeftApplication (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleBannerLeftApplication event received");
		}

        #endregion

        #region Interstitial callback handlers

		private void HandleInterstitialLoaded (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleInterstitialLoaded event received");
		}

		private void HandleInterstitialFailedToLoad (object sender, AdFailedToLoadEventArgs args)
		{
				//(Optional)Try to request new one
				MonoBehaviour.print ("HandleInterstitialFailedToLoad event received with message: " + args.Message);
		}

		private void HandleInterstitialOpened (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleInterstitialOpened event received");
		}

		private void HandleInterstitialClosed (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleInterstitialClosed event received");
				RequestInterstitialAd ();
		}

		private void HandleInterstitialLeftApplication (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleInterstitialLeftApplication event received");
		}

        #endregion

        #region RewardBasedVideo callback handlers

		private void HandleRewardBasedVideoLoaded (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleRewardBasedVideoLoaded event received");
		}

		private void HandleRewardBasedVideoFailedToLoad (object sender, AdFailedToLoadEventArgs args)
		{
				//(Optional)Try to request new one
				MonoBehaviour.print ("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
		}

		private void HandleRewardBasedVideoOpened (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleRewardBasedVideoOpened event received");
		}

		private void HandleRewardBasedVideoStarted (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleRewardBasedVideoStarted event received");
		}

		private void HandleRewardBasedVideoClosed (object sender, EventArgs args)
		{
				RequestRewardBasedVideoAd ();
				MonoBehaviour.print ("HandleRewardBasedVideoClosed event received");
		}

		private void HandleRewardBasedVideoRewarded (object sender, Reward args)
		{
				string type = args.Type;
				double amount = args.Amount;
				MonoBehaviour.print ("HandleRewardBasedVideoRewarded event received for " + amount.ToString () + " " + type);
		}

		private void HandleRewardBasedVideoLeftApplication (object sender, EventArgs args)
		{
				MonoBehaviour.print ("HandleRewardBasedVideoLeftApplication event received");
		}

        #endregion
		
#endif
    }
}
