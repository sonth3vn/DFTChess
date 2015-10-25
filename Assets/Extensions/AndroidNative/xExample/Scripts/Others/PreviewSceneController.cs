using UnityEngine;
using System.Collections;

public class PreviewSceneController : MonoBehaviour {



	public void SendMail() {
		AndroidSocialGate.SendMail("Send Mail", "", "Android Native Plugin Question", "stans.assets@gmail.com");
	}

	public void OpenDocs() {
		string url = "http://goo.gl/VmIFVQ";
		Application.OpenURL(url);
	}

	public void OpenAssetStore() {
		string url = "http://goo.gl/g8LWlC";
		Application.OpenURL(url);
	}


	public void MorePlugins() {
		string url = "http://goo.gl/MgEirV";
		Application.OpenURL(url);
	}


}
