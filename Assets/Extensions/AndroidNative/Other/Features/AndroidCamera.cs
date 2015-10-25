using UnityEngine;
using System;
using System.Collections;

public class AndroidCamera : SA_Singleton<AndroidCamera>  {


	//Actions
	public Action<AndroidImagePickResult> OnImagePicked;
	
	//Events
	public const string  IMAGE_PICKED = "image_picked";



	public void SaveImageToGalalry(Texture2D image) {
		if(image != null) {
			byte[] val = image.EncodeToPNG();
			string mdeia = System.Convert.ToBase64String (val);
			AndroidNative.SaveToGalalry(mdeia);
		}  else {
			Debug.LogWarning("AndroidCamera::SaveToGalalry:  image is null");
		}

	}


	public void SaveScreenshotToGallery() {
		SA_ScreenShotMaker.instance.OnScreenshotReady += OnScreenshotReady;
		SA_ScreenShotMaker.instance.GetScreenshot();
	}


	public void GetImageFromGallery() {
		AndroidNative.GetImageFromGallery();
	}
	
	
	
	public void GetImageFromCamera() {
		AndroidNative.GetImageFromCamera();
	}




	private void OnImagePickedEvent(string data) {

		Debug.Log("OnImagePickedEvent");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AndroidImagePickResult result =  new AndroidImagePickResult(storeData[0], storeData[1]);

		dispatch(IMAGE_PICKED, result);
		if(OnImagePicked != null) {
			OnImagePicked(result);
		}

	}



	private void OnScreenshotReady(Texture2D tex) {
		SA_ScreenShotMaker.instance.OnScreenshotReady -= OnScreenshotReady;
		SaveImageToGalalry(tex);

	}
}
