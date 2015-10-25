//#define SA_DEBUG_MODE
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////




using UnityEngine;
using System;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif


public class IOSCamera : ISN_Singleton<IOSCamera> {


	//Actions
	public Action<IOSImagePickResult> OnImagePicked;

	//Events
	public const string  IMAGE_PICKED = "image_picked";



	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISN_SaveToCameraRoll(string encodedMedia);

	[DllImport ("__Internal")]
	private static extern void _ISN_GetImageFromCamera();

	[DllImport ("__Internal")]
	private static extern void _ISN_GetImageFromAlbum();


	#endif





	public void SaveTextureToCameraRoll(Texture2D texture) {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			string bytesString = System.Convert.ToBase64String (val);
			_ISN_SaveToCameraRoll(bytesString);
		} 
		#endif
	}


	public void SaveScreenshotToCameraRoll() {
		StartCoroutine(SaveScreenshot());
	}


	public void GetImageFromCamera() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_GetImageFromCamera();
		#endif
	}

	public void GetImageFromAlbum() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
		_ISN_GetImageFromAlbum();
		#endif
	}



	private void OnImagePickedEvent(string data) {


		IOSImagePickResult result =  new IOSImagePickResult(data);



		dispatch(IMAGE_PICKED, result);
		if(OnImagePicked != null) {
			OnImagePicked(result);
		}


	}

	
	private IEnumerator SaveScreenshot() {
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		SaveTextureToCameraRoll(tex);
		
		Destroy(tex);
		
	}
}
