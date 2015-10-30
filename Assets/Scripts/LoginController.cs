using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJson;
using Pomelo.DotNetClient;

public class LoginController : MonoBehaviour {
	public HomeController homeController;
	GameManager manager;
	[Header("HUD")]
	public InputField txtUserName;
	public InputField txtPassword;
	
	// Param pomelo
	public static string channel = "";
	string userName;
	string password;

	public IList lsServer;

	PomeloClient pclient = null;

	bool isLogin = false;

	// Use this for initialization
	void Start () {
		manager = GameManager.instance;
		pclient = GameManager._pclient;
		StartCoroutine (ListenLoginAction());
	}

	IEnumerator ListenLoginAction(){
		while (!isLogin)
		{
			yield return 0; // wait for next frame
		}

		GameManager.instance._lsServer = lsServer;
		GameManager.instance.ChangeState (GameState.kStateLobby);
		Application.LoadLevel("LobbyScene");
	}
	
	public void LoginWithAccount(){
		if (txtUserName.text.Length == 0 || txtPassword.text.Length == 0) {
			switch (Application.platform) {
				case RuntimePlatform.Android:
					AndroidDialog androidRate = AndroidDialog.Create (Constants.ERROR, Constants.LOGIN_NOT_ENOUGH);
					androidRate.addEventListener (BaseEvent.COMPLETE, manager.OnAndroidRate);
					break;
				case RuntimePlatform.IPhonePlayer:
					IOSDialog iOSRate = IOSDialog.Create (Constants.ERROR, Constants.LOGIN_NOT_ENOUGH);
					iOSRate.addEventListener (BaseEvent.COMPLETE, manager.OniOSdRate);
					break;
				case RuntimePlatform.WP8Player:
					WP8Dialog wp8Rate = WP8Dialog.Create (Constants.ERROR, Constants.LOGIN_NOT_ENOUGH);
					wp8Rate.addEventListener (BaseEvent.COMPLETE, manager.OnWP8Rate);
					break;
				default:
					Debug.LogError("Username va password khong duoc bo trong!");
					break;
			}	
			return;
		}
//		JsonObject userLogin = new JsonObject();
//		userLogin ["name"] = txtUserName.text;
//		userLogin ["password"] = txtPassword.text;
//		userLogin ["channelId"] = "server-1";

		WWWForm formLogin = new WWWForm();
		formLogin.AddField("name", txtUserName.text);
		formLogin.AddField("password", txtPassword.text);
		WWW loginHTTP = new WWW(Constants.URLLOGIN, formLogin);
		StartCoroutine(WaitForLoginRequest(loginHTTP));

	}

	public void LoginWithFacebook(){
	
	}
	
	IEnumerator WaitForLoginRequest(WWW www)
	{
		yield return www;
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
			GameManager.instance._user.userID = www.text.Replace("\"", "");
			pclient = GameManager._pclient;
			if (pclient != null){
				pclient.connect(null, data =>
				{
					Debug.Log ("Connect server ok ...");
					JsonObject msg  = new JsonObject();
					pclient.request(Constants.GATESERVER, msg, OnResponseFromGate);
				});
			}
			else{
				Debug.LogError("Pomelo client null");
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}  

	} 

	public void OnResponseFromGate(JsonObject result){
		if(System.Convert.ToInt32(result["code"]) == 200){
			Debug.Log ("Connect gate ok...");
			Debug.Log (result);

			string jsonServer = System.Convert.ToString(result["server"]);
			lsServer = Util.DeserializeJsonArrayToList(jsonServer);
			isLogin = true;
		}
		else{
			Debug.LogError("Error code: " + System.Convert.ToInt32(result["code"]));
		}
	}

	public void Register(){
		homeController.loginPanel.transform.localScale = Vector3.zero;
		homeController.regPanel.transform.localScale = Vector3.one;
	}

}
