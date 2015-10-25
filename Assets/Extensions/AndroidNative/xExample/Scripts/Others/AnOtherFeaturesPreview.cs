using UnityEngine;
using System.Collections;

public class AnOtherFeaturesPreview : MonoBehaviour {

	public GameObject image;
	public Texture2D helloWorldTexture;



	void Awake() {
		AndroidCamera.instance.OnImagePicked += OnImagePicked;
	}

	public void SaveToGalalry() {
		AndroidCamera.instance.SaveImageToGalalry(helloWorldTexture);
	}

	public void SaveScreenshot() {
		AndroidCamera.instance.SaveScreenshotToGallery();
	}


	public void GetImageFromGallery() {
		AndroidCamera.instance.GetImageFromGallery();
	}
	
	
	
	public void GetImageFromCamera() {
		AndroidCamera.instance.GetImageFromCamera();
	}






	private void EnableImmersiveMode() {
		ImmersiveMode.instance.EnableImmersiveMode();
	}
	



	private void LoadAppInfo() {

		AndroidAppInfoLoader.instance.addEventListener (AndroidAppInfoLoader.PACKAGE_INFO_LOADED, OnPackageInfoLoaded);
		AndroidAppInfoLoader.instance.LoadPackageInfo ();
	}



	private void OnImagePicked(AndroidImagePickResult result) {
		Debug.Log("OnImagePicked");
		if(result.IsSucceeded) {
			image.renderer.material.mainTexture = result.image;
		}
	}

	private void OnPackageInfoLoaded() {
		AndroidAppInfoLoader.instance.removeEventListener (AndroidAppInfoLoader.PACKAGE_INFO_LOADED, OnPackageInfoLoaded);

		string msg = "";
		msg += AndroidAppInfoLoader.instance.PacakgeInfo.versionName + "\n";
		msg += AndroidAppInfoLoader.instance.PacakgeInfo.versionCode + "\n";
		msg += AndroidAppInfoLoader.instance.PacakgeInfo.packageName + "\n";
		msg += System.Convert.ToString(AndroidAppInfoLoader.instance.PacakgeInfo.lastUpdateTime) + "\n";
		msg += AndroidAppInfoLoader.instance.PacakgeInfo.sharedUserId + "\n";
		msg += AndroidAppInfoLoader.instance.PacakgeInfo.sharedUserLabel;

		AndroidNative.showMessage("App Info Loaded", msg);
	}

}
