using UnityEngine;
using System.Collections;

public class CustomInterstisialExample : MonoBehaviour {






	void Start () {
		GoogleMobileAd.Init();


		GoogleMobileAd.controller.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_LOADED, OnInterstisialsLoaded);
		GoogleMobileAd.controller.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, OnInterstisialsOpen);

		GoogleMobileAd.controller.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_CLOSED, OnInterstisialsClosed);

		//loadin ad:
		GoogleMobileAd.LoadInterstitialAd ();
	}

	private void OnInterstisialsLoaded() {
		//ad loaded, strting ad
		GoogleMobileAd.ShowInterstitialAd ();

	}

	private void OnInterstisialsOpen() {
		//pausing the game
	}

	private void OnInterstisialsClosed() {
		//un-pausing the game
	}

}
