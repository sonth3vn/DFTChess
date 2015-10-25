using UnityEngine;
using System.Collections;

public class AndroidNativeUtility : SA_Singleton<AndroidNativeUtility> {

	public static string PACKAGE_FOUND 		= "package_found";
	public static string PACKAGE_NOT_FOUND	=  "package_not_found";

	
	//--------------------------------------
	// Init
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------
	
	
	public void CheckIsPackageInstalled(string packageName) {
		AndroidNative.isPackageInstalled(packageName);

	}

	public void LoadGoogleAccountNames() {
		AndroidNative.loadGoogleAccountNames();
	}



	//--------------------------------------
	// Static Methods
	//--------------------------------------

	public static void ShowPreloader(string title, string message) {
		AndroidNative.ShowPreloader(title, message);
	}
	
	public static void HidePreloader() {
		AndroidNative.HidePreloader();
	}


	public static void OpenAppRatingPage(string url) {
		AndroidNative.OpenAppRatePage(url);
	}


	//--------------------------------------
	// Events
	//--------------------------------------

	private void OnPacakgeFound(string packageName) {
		dispatch(PACKAGE_FOUND, packageName);
	}

	private void OnPacakgeNotFound(string packageName) {
		dispatch(PACKAGE_NOT_FOUND, packageName);
	}




}

