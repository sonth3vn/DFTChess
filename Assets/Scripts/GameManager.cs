using UnityEngine;
using System.Collections;
using Pomelo.DotNetClient;
using SimpleJson;

public class GameManager : SA_Singleton<GameManager> {
	public static PomeloClient _pclient;
	
	//Lobby Controller
	public LobbyController _lobby;

	//Game Controller
	public GameController _gc;

	//Room Controller
	public RoomController _rc;

	[Header("Share")]
	public Users user;
	public IList _lsServer;
	public GameState _gameState;
	public JsonObject _loginUser;
	public string roomData;

	public Table _table = new Table();

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (gameObject);
	}

	void Start(){
		Users user = new Users ();
		GameManager.instance.user = user;
		_gameState = GameState.kStateLoading;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape)){
			PressedBack();
		}
	}

	public void PressedBack(){
		string nameScene = Application.loadedLevelName;
		string nextScene = "HomeScene";
		if (nameScene.Equals("LobbyScene")){
			_gameState = GameState.kStateHome;
		}
		else if (nameScene.Equals("RoomScene")){
			_gameState = GameState.kStateLobby;
			nextScene = "LobbyScene";
			//Request to pomelo
		}
		else if (nameScene.Equals("GameScene")){
			_gameState = GameState.kStateJoinRoom;
			nextScene = "RoomScene";
			//Request to pomelo
			JsonObject param  = new JsonObject();
			param["room"] = _table.roomID;
			_pclient.request (Constants.EXITTABLE, param, OnQueryExitTable);
		}
		Application.LoadLevel(nextScene);
	}

	public void PressedJoinServer(Server server){
		if (_pclient != null) {
			Debug.Log ("Connect server ok ...");
			IDictionary testServer = _lsServer[0] as IDictionary;
			_pclient.disconnect();
			string sname = System.Convert.ToString(testServer["name"]);
			string sid = System.Convert.ToString(testServer["id"]);
			string host = System.Convert.ToString(testServer["host"]);
			int port = System.Convert.ToInt32(testServer["clientPort"]);
			Debug.Log ("host: " + host + ", port: " + port);
			_pclient.initClient(host, 
			                    port, 
			                    () => {
				Debug.Log("Connect host connector ok ...");	
				_pclient.connect(null, data =>
				                 {
					Debug.Log ("Try to login ...");
					JsonObject userLogin  = new JsonObject();
					userLogin["uid"] = user.userID;
					userLogin["sid"] = sid;
					userLogin["sname"] = sname;
					_pclient.request (Constants.LOGINSERVER, userLogin, this.OnQueryLogin);
					
				});
				
			});
			//						_pclient.on("onStatus", (dataStatus) => {
			//							Debug.Log ("onStatus: " + dataStatus);
			//						});
//			JsonObject dataRequest = new JsonObject();
//			dataRequest["room"] = table._name.Replace("r-", "");
//			_pclient.request ("chinesechess.chinesechessHandler.createorjoin", dataRequest, OnResponseJoin);
		}
		else {
			Debug.LogError ("pClient null");
		}
	}

	public void PressedJoinTable(TableController table){
		if (_pclient != null) {
			Debug.Log ("Try to join table ...");
			JsonObject room  = new JsonObject();
			room["room"] = table.table.roomID;
			Debug.Log (table.table.roomID);
			_pclient.request (Constants.JOINTABLE, room, this.OnQueryJoinTable);
		}
		else {
			Debug.LogError ("pClient null");
		}

		//sonth
		_pclient.on("onStatus", (data) => {
			IList rooms = Util.DeserializeJsonArrayToList(System.Convert.ToString(data["rooms"]));
			IDictionary room = rooms[0] as IDictionary;
			_table.roomID = System.Convert.ToString(room["id"]);
			_table.status = System.Convert.ToString(room["status"]);
			_table.ruby = System.Convert.ToInt32(room["ruby"]);
			_table.type = System.Convert.ToInt32(room["type"]);
			_table.time = System.Convert.ToInt32(room["time"]);
			
			IDictionary jsonHost = room["host"] as IDictionary;
			Player host = _table.host;
			host.name = System.Convert.ToString(jsonHost["name"]);
			host.uid = System.Convert.ToString(jsonHost["uid"]);
			host.sid = System.Convert.ToString(jsonHost["sid"]);
			host.roomName = System.Convert.ToString(jsonHost["rname"]);
			host.avatar = System.Convert.ToString(jsonHost["avatar"]);
			host.star = System.Convert.ToInt32(jsonHost["star"]);
			host.ruby = System.Convert.ToInt32(jsonHost["ruby"]);
			host.roomID = System.Convert.ToString(jsonHost["room"]);
			
			IDictionary jsonGuest = room["guest"] as IDictionary;
			if (jsonGuest.Contains("uid")){
				// guest not null
				Player guest = _table.guest;
				guest.name = System.Convert.ToString(jsonGuest["name"]);
				guest.uid = System.Convert.ToString(jsonGuest["uid"]);
				guest.sid = System.Convert.ToString(jsonGuest["sid"]);
				guest.roomName = System.Convert.ToString(jsonGuest["rname"]);
				guest.avatar = System.Convert.ToString(jsonGuest["avatar"]);
				guest.star = System.Convert.ToInt32(jsonGuest["star"]);
				guest.ruby = System.Convert.ToInt32(jsonGuest["ruby"]);
				guest.roomID = System.Convert.ToString(jsonGuest["room"]);
			}
		});
	}

	public void PressedStartGame(){
		if (_pclient != null) {
			Debug.Log("Send request start game ...");
			JsonObject param  = new JsonObject();
			_pclient.request (Constants.STARTGAME, param, this.OnQueryStartGame);
		}
		else {
			Debug.LogError ("pClient null");
		}
	}

	public void OnQueryLogin(JsonObject result){
		int code = System.Convert.ToInt32 (result ["code"]);
		if (code == 200) {
			Debug.Log (result);
			roomData = System.Convert.ToString(result["data"]);
			_lobby.isJoin = true;
		}
		else{
			Debug.Log ("OnQueryLogin, code: " + code);
		}
	}

	public void OnQueryJoinTable(JsonObject result){
		int code = System.Convert.ToInt32 (result ["code"]);
		if (code == 200) {
			_rc.isPlay = true;
		}
		else{
			Debug.Log ("OnQueryJoinTable, code: " + code);
		}
	}

	public void OnQueryExitTable(JsonObject result){
		int code = System.Convert.ToInt32 (result ["code"]);
		if (code == 200) {
			Debug.Log (result);
		}
		else{
			Debug.Log ("OnQueryExitTable, code: " + code);
		}
	}

	public void OnQueryStartGame(JsonObject result){
		int code = System.Convert.ToInt32 (result ["code"]);
		if (code == 200) {
			//Create board
			Debug.Log (result["board"].ToString());
			_gc.CreateMap(result["board"].ToString());
		}
		else{
			Debug.Log ("OnQueryStartGame, code: " + code);
		}
	}
	

//	public void OnResponseFromGate(JsonObject result){
//		if(System.Convert.ToInt32(result["code"]) == 200){
//			Debug.Log ("Connect gate ok...");
//			string jsonServer = System.Convert.ToString(result["server"]);
//			IList server = Util.DeserializeJsonArrayToList(jsonServer);
//			GameManager.instance._lsServer = server;
//			IDictionary testServer = server[0] as IDictionary;
//			pclient.disconnect();
//			string sname = System.Convert.ToString(testServer["name"]);
//			string sid = System.Convert.ToString(testServer["id"]);
//			string host = System.Convert.ToString(testServer["host"]);
//			int port = System.Convert.ToInt32(testServer["clientPort"]);
//			Debug.Log ("host: " + host + ", port: " + port);
//			pclient.initClient(host, 
//			                   port, 
//			                   () => {
//				Debug.Log("Connect host connector ok ...");	
//				pclient.connect(null, data =>
//				                {
//					Debug.Log ("Try to login ...");
//					JsonObject userLogin  = new JsonObject();
//					userLogin["uid"] = _userID;
//					userLogin["sid"] = sid;
//					userLogin["sname"] = sname;
//					pclient.request ("connector.ConnectorHandler.login", userLogin, this.OnQueryLogin);
//					
//				});
//				
//			});
//			//						_pclient.on("onStatus", (dataStatus) => {
//			//							Debug.Log ("onStatus: " + dataStatus);
//			//						});
//		}
//		else{
//			Debug.LogError("Error code: " + System.Convert.ToInt32(result["code"]));
//		}
//	}

//	public void OnResponseJoin(JsonObject result){
//		if (System.Convert.ToInt32 (result ["code"]) == 200) {
//			Debug.Log(result);
//			JsonObject host = (JsonObject)result["host"];
//			user.name = System.Convert.ToString(host["name"]);
//			user.avatar = System.Convert.ToString(host["avatar"]);
//			_loginUser = result;
//
//			_lobby.isJoin = true;
//		}
//
//	}

	public void ChangeState(GameState newState){
		if (newState == _gameState) {
			return;
		}

		if (newState == GameState.kStateJoinRoom) {
			// Check status phong

		}
		_gameState = newState;
	}

	//When quit, release resource
	void OnApplicationQuit(){
		if (_pclient != null) {
			_pclient.disconnect();
		}
	}

	// Callback
	public void OnWP8Rate (CEvent e)
	{
		//removing listner
		e.dispatcher.removeEventListener (BaseEvent.COMPLETE, OnWP8Rate);
	}
	
	public void OniOSdRate (CEvent e)
	{		
		(e.dispatcher as IOSRateUsPopUp)
			.removeEventListener (BaseEvent.COMPLETE, OniOSdRate);
	}
	
	public void OnAndroidRate (CEvent e)
	{
		(e.dispatcher as
		 AndroidRateUsPopUp).removeEventListener (BaseEvent.COMPLETE, OnAndroidRate);
	}

}
