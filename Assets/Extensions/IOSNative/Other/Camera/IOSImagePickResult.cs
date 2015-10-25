using UnityEngine;
using System.Collections;

public class IOSImagePickResult : ISN_Result {


	private Texture2D _image = null;

	public IOSImagePickResult(string ImageData):base(true) {
		if(ImageData.Length == 0) {
			_IsSucceeded = false;
			return;
		}

		
		byte[] decodedFromBase64 = System.Convert.FromBase64String(ImageData);
		_image = new Texture2D(1, 1, TextureFormat.DXT5, false);
		_image.LoadImage(decodedFromBase64);

	}
	


	public Texture2D image {
		get {
			return _image;
		}
	}
}
