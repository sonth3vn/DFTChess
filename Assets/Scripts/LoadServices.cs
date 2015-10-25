using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pomelo.DotNetClient;
using SimpleJson;

public class LoadServices : SA_Singleton<LoadServices> {

	string url = "https://google.com";

	[Header("Pomelo")]
	public string host= "127.0.0.1";
	public int port=3014;
	PomeloClient _pclient = null;

	bool isConnected = false;

	void Awake(){
		//DontDestroyOnLoad (gameObject);
		//ActivateFB
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer){
			SPFacebook.instance.addEventListener (FacebookEvents.FACEBOOK_INITED, OnInit);
			if (!SPFacebook.instance.IsInited){
				SPFacebook.instance.Init ();
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		StartCoroutine (StartLoadServices());
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Escape)) {
//			if (_pclient != null) {
//				_pclient.disconnect();
//			}
			Application.Quit();
		}
	}
	
	IEnumerator StartLoadServices(){
		WWW www = new WWW (url);
		yield return www;
		if (www.isDone && www.bytesDownloaded > 0) {
			// connection internet
//			// Setup IAP Store
//			if (Application.platform == RuntimePlatform.Android){
//				AndroidIAPManager.init();
//			}
//			else if (Application.platform == RuntimePlatform.IPhonePlayer){
//				IOSIAPManager.init();
//			}
//			else{
//				Debug.Log ("Any other platform!");
//			}
			Debug.Log ("Connection OK");

			_pclient = GameManager._pclient;
			if (_pclient == null){
				_pclient = new PomeloClient();
				GameManager._pclient = _pclient;
			}

			//listen on network state changed event
			_pclient.NetWorkStateChangedEvent += (state) =>
			{
				Debug.Log (state);
			};
			Debug.Log ("Try to init ...");
			_pclient.initClient(host, port, () =>
			{
				Debug.Log ("Try to connect ...");
				isConnected = true;
			});
		}
		
		if (www.isDone && www.bytesDownloaded == 0) {
			Debug.LogError("No connection");
		}

		while (!isConnected){
			yield return 0;
		}

		GameManager.instance.ChangeState(GameState.kStateHome);
	}

//	public void OnResponseFromGate(JsonObject result){
//		if(System.Convert.ToInt32(result["code"]) == 200){
//			Debug.Log ("Connect gate ok...");
//			string jsonServer = System.Convert.ToString(result["server"]);
//			IList server = Util.DeserializeJsonArrayToList(jsonServer);
//			IDictionary testServer = server[0] as IDictionary;
//			_pclient.disconnect();
//			Debug.Log (System.Convert.ToString(testServer["host"]));
//			Debug.Log (System.Convert.ToString(testServer["port"]));
//			_pclient.initClient(System.Convert.ToString(testServer["host"]), 
//			                    System.Convert.ToInt32(testServer["port"]), 
//			                    () => {
//				Debug.Log("Connect host connector ok ...");
//				isConnected = true;
//			});
////			_pclient.on("onStatus", (dataStatus) => {
////				Debug.Log ("onStatus: " + dataStatus);
////			});
//		}
//	}
	
	private void OnInit ()
	{			
		FB.ActivateApp ();	
	}
}
