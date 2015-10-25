using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using Pomelo.DotNetClient;

public class RegisterController : MonoBehaviour {
	public HomeController homeController;
	[Header("HUD")]
	public InputField txtEmail;
	public InputField txtPassword;
	public InputField txtConfirm;
	public Toggle toggleAgree;

	// Use this for initialization
	void Start () {
	
	}

	public void RegisterWithAccount(){
		string email = txtEmail.text;
		string password = txtPassword.text;
		string confirm = txtConfirm.text;
		if (email.Length == 0 || password.Length == 0 || confirm.Length == 0) {
			Debug.LogError("Khong duoc de trong!");
			return;
		}

		if (!password.Equals (confirm)) {
			Debug.LogError("Mat khau khong trung khop!");
			return;
		}

		if (!toggleAgree.isOn) {
			Debug.LogError ("Chua dong y dieu khoan");
			return;
		}

		int iRand = Random.Range (0, 27);
		string avatar = @"/assets/images/avatar/" + (iRand < 10 ? "00" + iRand : "0" + iRand) + ".png";

		WWWForm formReg = new WWWForm();
		formReg.AddField("name", email);
		formReg.AddField("email", email);
		formReg.AddField("password", password);
		formReg.AddField ("avatar", avatar);
		WWW regHTTP = new WWW(Constants.URLREG, formReg);
		StartCoroutine(WaitForRegRequest(regHTTP));
	}

	IEnumerator WaitForRegRequest(WWW www)
	{
		yield return www;
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
			Login (); //Quay ve man hinh dang nhap
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}  
	} 
	
	public void Login(){
		homeController.regPanel.transform.localScale = Vector3.zero;
		homeController.loginPanel.transform.localScale = Vector3.one;
	}
}
