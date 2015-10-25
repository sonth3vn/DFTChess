using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using UnityEngine.UI;
using Pomelo.DotNetClient;

public enum GameplayState{
	kGameplayWaiting = 0,
	kGameplayPlaying = 1,
	kGameplayEndgame = 2,
};

public class GameController : SA_Singleton<GameController> {
	GameManager gameManager;
	PomeloClient pomelo;
	public EndgameController egController;
	public Transform camera;
	bool isShowTmpPanel = false;
	public GameplayState gameState;

	[HideInInspector] public bool _isSelected;
	[HideInInspector] public ChessController _chessSelected;
	[HideInInspector] public PlayerColor _color;
	[HideInInspector] public List<GameObject> _lsCheckmated;
	[HideInInspector] public List<ChessController> _lsChess;
//	public JsonObject v;
//	public string _uid;
//	public string _room;

	public Image _select;
	public Image _dangerous;
	public GameObject btnStart;

	MapCreator creator;
	bool isCreate = false; //create map
	bool isMove = false; //pomelo.on("onMoved")
	bool isShowHidden = false;
	bool _createMate = false; //pomelo.on("onPath")
	bool _isCaptured = false; //pomelo.on("Capture")
	bool _isDangerous = false; //pomelo.on("onDangerous")
	bool _isNotDangerous = false; //pomelo.on("onNotDangerous")
	bool _hasMove = false; //Callback after move
	bool _isEndgame = false;
	bool _isOtherExit = false;
	Point pCoord = new Point(); //Position goc
	Point pPiece = new Point(); //Position dich
	Point pCaptured = new Point(); //Position chess bi an 
	Point pDanger = new Point();
	Point pHidden = new Point ();
	string nameChessHidden;
	string nameChessDestroy;
	string nameChessCaptured;
	EndgameType typeEndgame;
	IList _lsRule;
	string _boardData;
	Table _table;
	
	[Header("ChessBoard")]
	public GameObject StartPoint;
	public Image imgUser;
	public Image imgGuest;

	[Header("Public")]
	public Sprite sourceHost;
	public Sprite sourceGuest;

	[Header("TableInfo")]
	public Text lblCoin;
	public Text lblTableTimeMinutes;
	public Text lblTableName;

	public Text lblOtherPName;
	public Text lblOtherPLevel;
	public Text lblOtherPCoin;
	public Text lblOtherPWin;
	public Text lblOtherPTimeM;
	public Text lblOtherPTimeS;
	public Image imgOtherPAvartar;

	public Text lblPName;
	public Text lblPLevel;
	public Text lblPCoin;
	public Text lblPWin;
	public Text lblPTimeM;
	public Text lblPTimeS;
	public Image imgPAvartar;

	void Start(){
		gameManager = GameManager.instance;
		gameManager._gc = this;
		pomelo = GameManager._pclient;
		creator = GetComponent<MapCreator> ();
		_isSelected = false;
		_chessSelected = null;
		_color = PlayerColor.kPlayerColorHost;
		_table = GameManager.instance._table;
		if (_table.guest != null) {
			_color = PlayerColor.kPlayerColorGuest;
		}

		Util.loadPictureCB = LoadPictureCallback;
		SetupLabel ();
		PomeloListen ();
	}

	void SetupLabel(){
		lblCoin.text = @"" + _table.ruby;
		lblTableTimeMinutes.text = @"" + (_table.time / 60);
//		lblTableName.text = @"" + _table.roomID;
		Player player = _table.host;
		Player otherPlayer = _table.guest;
		if (_color == PlayerColor.kPlayerColorGuest) {
			player = _table.guest;
			otherPlayer = _table.host;
		}

		lblPName.text = player.name;
		lblPLevel.text = @"" + player.star;
		lblPCoin.text = @"" + player.ruby;
//		lblPWin.text = @"" + 1;
//		lblPTimeM.text = @"" + 1;
//		lblPTimeS.text = @"" + 1;
		string wwwPath = Constants.DOMAIN + player.avatar;
		Util.instance.LoadPictureFromURL (wwwPath, "player");

		if (otherPlayer != null) {
			lblOtherPName.text = otherPlayer.name;
			lblOtherPLevel.text = @"" + otherPlayer.star;
			lblOtherPCoin.text = @"" + otherPlayer.ruby;
			//		lblOtherPWin.text = @"" + 1;
			//		lblOtherPTimeM.text = @"" + 1;
			//		lblOtherPTimeS.text = @"" + 1;
			wwwPath = Constants.DOMAIN + otherPlayer.avatar;
			Util.instance.LoadPictureFromURL (wwwPath, "otherPlayer");
		}
	}

	void LoadPictureCallback(Texture2D result, string player){
		if (result == null) {
			return;
		}
		if (player.Equals ("player")) {
			imgPAvartar.sprite = Sprite.Create (result, imgPAvartar.sprite.rect, new Vector2(0.5f, 0.5f));
		}
		else{
			imgOtherPAvartar.sprite = Sprite.Create (result, imgPAvartar.sprite.rect, new Vector2(0.5f, 0.5f));
		}
	}

	void Update(){
		if (isCreate) {
			isCreate = false;
			creator.playerColor = _color;
			creator.CreateMap (_boardData);
		}

		if (isMove) {
			isMove = false;
			ChessController chessCaptured = FindChessWithPiece(pPiece);
			nameChessDestroy = chessCaptured != null ? chessCaptured._name : null;	
			ChessController chessMove = FindChessWithPiece(pCoord);
			Move(chessMove, pPiece);
		}

		if (isShowHidden) {
			isShowHidden = false;
			ShowHiddenChess(pHidden, nameChessHidden);
		}

		if (_createMate) {
			_createMate = false;
			foreach (var obj in _lsRule) {
				IDictionary rule = obj as IDictionary;
				Point pos = new Point{ x = System.Convert.ToInt32(rule["x"]), y = System.Convert.ToInt32(rule["y"])};
				if (_color == PlayerColor.kPlayerColorGuest){
					pos.y = 9 - pos.y;
				}
				creator.CreateCheckMate(pos);			
			}
		}
			
		if (_hasMove) {
			_hasMove = false;
			Move (_chessSelected, pCaptured);
		}

		if (_isCaptured) {
			_isCaptured = false;
			Captured(pCaptured);
		}

		if (_isNotDangerous) {
			_isNotDangerous = false;
			_dangerous.transform.localScale = Vector3.zero;
		}
		else{
			if (_isDangerous) {
				_isDangerous = false;
				Dangerous(pDanger);
			}
		}

		if (_isEndgame) {
			_isEndgame = false; //reset
			gameState = GameplayState.kGameplayEndgame;
			egController.Endgame(typeEndgame);
		}

		if (_isOtherExit) {
			_isOtherExit = false;
			egController.Reset();
		}
	}

	void PomeloListen(){
		//Handle events
		pomelo.on("onChess", (data) => {
			Debug.Log (data);
			string cmd = System.Convert.ToString(data["cmd"]);
			switch (cmd) {
				case "onWin":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}
					_isEndgame = true;
					typeEndgame = EndgameType.kEndgameTypeWin;
					break;			
				}
				case "onLose":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}
					_isEndgame = true;
					typeEndgame = EndgameType.kEndgameTypeLose;
					break;
				}
				case "countDown":
					break;
				case "guestJoin":
					Debug.Log ("guestJoin");
					Debug.Log (data);
					break;
				case "playerExit":
					if (gameState != GameplayState.kGameplayPlaying){
						//Reset cac label
					}
					else{
						_isOtherExit = true;
					}
					break;
				case "chessBegin":
					//Player start
					break;
				case "gameStarted":
					gameState = GameplayState.kGameplayPlaying;
					break;
				case "chessMoved":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}
					JsonObject dCoord = (JsonObject)data["coord"];
					JsonObject dPiece = (JsonObject)data["piece"];
					pCoord.x = System.Convert.ToInt32(dCoord["x"]); pCoord.y = System.Convert.ToInt32(dCoord["y"]);
					pPiece.x = System.Convert.ToInt32(dPiece["x"]); pPiece.y = System.Convert.ToInt32(dPiece["y"]);
					
					isMove = true;
					break;
				}
				case "chessCaptured":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}//sonth1
					JsonObject jsonCapture = (JsonObject)data["captured"];
					JsonObject jsonPos = (JsonObject)jsonCapture["pos"];
					pCaptured = new Point{x = System.Convert.ToInt32(jsonPos["x"]), y = System.Convert.ToInt32(jsonPos["y"])};
//					nameChessDestroy = System.Convert.ToString(jsonCapture["name"]);
					nameChessCaptured = System.Convert.ToString(jsonCapture["name"]);
					_isCaptured = true;
					break;

				}
				case "onDangerous":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}
					JsonObject jsonPosDanger = (JsonObject)data["pos"];
					pDanger.x = System.Convert.ToInt32(jsonPosDanger["x"]); pDanger.y = System.Convert.ToInt32(jsonPosDanger["y"]);
					_isDangerous = true;	
					break;
				}
				case "onNotDangerous":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}
					_isNotDangerous = true;
					break;
				}
				case "onPath":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}
					CreatePath(System.Convert.ToString(data["nRule"]));
					break;

				}
				case "onShowHidden":{
					if (gameState != GameplayState.kGameplayPlaying){
						break;
					}
					JsonObject dataObj = (JsonObject)data["data"];
					nameChessHidden = System.Convert.ToString (dataObj["name"]);
					JsonObject posObj = (JsonObject)dataObj["pos"];
					pHidden.x = System.Convert.ToInt32(posObj["x"]);
					pHidden.y = System.Convert.ToInt32(posObj["y"]);
					isShowHidden = true;
					break;
				}
			}
		});
	}

	public void CreateMap(string boardData){
		_boardData = boardData;
		isCreate = !isCreate;
	}

	/* --- Helper method --- */
	public void PressedBtnStart(GameObject sender){
		if (gameState != GameplayState.kGameplayWaiting)
			return;
		sender.SetActive (false);
		GameManager.instance.PressedStartGame ();
		// Neu chua co ai -> Chuyen sang che do cho
		// Neu da co chu phong -> Chuyen sang load ban co
	}

	public void SelectChess(int chessColor, Point chessPos){
		if (gameState != GameplayState.kGameplayPlaying)
			return;
		// Send request len pomelo server
		Debug.Log ("SelectChess");
		foreach (var obj in _lsCheckmated) {
			Destroy(obj.gameObject);
		}
		_lsCheckmated.Clear ();

//		if (_color == PlayerColor.kPlayerColorGuest) {
//			chessPos.y = 9 - chessPos.y;
//		}
		JsonObject param  = new JsonObject();
		param ["color"] = chessColor;
		JsonObject paramPos = new JsonObject ();
		paramPos ["x"] = chessPos.x;
		paramPos ["y"] = chessPos.y;
		param ["coord"] = paramPos;
		pomelo.request (Constants.SELECTCHESS, param, this.OnQuerySelectChess);
	}

	public void MoveToPath(Point targetPos){
		JsonObject param  = new JsonObject();
		param ["color"] = _chessSelected.color % 10;
		JsonObject paramCoord = new JsonObject ();
		paramCoord ["x"] = _chessSelected.currentPos.x;
		paramCoord ["y"] = _chessSelected.currentPos.y;
		param ["coord"] = paramCoord;
		JsonObject paramPiece = new JsonObject ();
		paramPiece ["x"] = targetPos.x;
		paramPiece ["y"] = targetPos.y;
		param ["piece"] = paramPiece;
		param ["name"] = _chessSelected._name;
		pomelo.request (Constants.MOVECHESS, param, this.OnQueryMoveToPath);
	}

	public void OnQuerySelectChess(JsonObject result){
		int code = System.Convert.ToInt32 (result ["code"]);
		if (code == 200) {
			Debug.Log ("Code 200, OK!");
			Debug.Log (result);
			//CreatePath(System.Convert.ToString(result["nRule"]));
		}
		else{
			Debug.Log ("OnQuerySelectChess, code: " + code);
			if (code == 500){
				string msg = System.Convert.ToString(result["msg"]);
				switch (Application.platform) {
				case RuntimePlatform.Android:
					AndroidDialog androidRate = AndroidDialog.Create (Constants.ERROR, msg, Constants.OK, null);
					androidRate.addEventListener (BaseEvent.COMPLETE, gameManager.OnAndroidRate);
					break;
				case RuntimePlatform.IPhonePlayer:
					IOSDialog iOSRate = IOSDialog.Create (Constants.ERROR, msg);
					iOSRate.addEventListener (BaseEvent.COMPLETE, gameManager.OniOSdRate);
					break;
				case RuntimePlatform.WP8Player:
					WP8Dialog wp8Rate = WP8Dialog.Create (Constants.ERROR, msg);
					wp8Rate.addEventListener (BaseEvent.COMPLETE, gameManager.OnWP8Rate);
					break;
				default:
					break;
				}	
			}
		}
	}

	public void OnQueryMoveToPath(JsonObject result){
		int code = System.Convert.ToInt32 (result ["code"]);
		if (code == 200) {
			Debug.Log (result);
//			Debug.Log ("Code 200, OK!");
//			Debug.Log (result);
//			JsonObject jsonPiece = (JsonObject)result["piece"];
//			pCaptured.x = System.Convert.ToInt32(jsonPiece["x"]);
//			pCaptured.y = System.Convert.ToInt32(jsonPiece["y"]);
//			_hasMove = true;
		}
		else{
			Debug.Log ("OnQueryMoveToPath, code: " + code);
		}
	}

	public void OnQueryShowHiddenChess(JsonObject result){
		int code = System.Convert.ToInt32 (result ["code"]);
		if (code == 200) {
			Debug.Log ("Code 200, OK!");
			Debug.Log (result);
			//CreatePath(System.Convert.ToString(result["nRule"]));
		}
		else{
			Debug.Log ("OnQueryShowHiddenChess, code: " + code);
		}
	}

	void CreatePath(string data){
//		foreach (var obj in _lsCheckmated) {
//			Destroy(obj.gameObject);
//		}
//		_lsCheckmated.Clear ();
		_lsRule = Util.DeserializeJsonArrayToList (data);
		_createMate = true;
	}

	public void Move(ChessController chess, Point pos){
		foreach (var obj in _lsCheckmated) {
			Destroy(obj.gameObject);
		}
		_lsCheckmated.Clear ();

		Transform startObject = creator.startObject;
		float range = creator.range;
		Point pMove = new Point{x = pos.x, y = pos.y};
		if (_color == PlayerColor.kPlayerColorGuest) {
			pMove.x = 8 - pos.x;
		}
		chess.transform.localPosition = new Vector3(startObject.localPosition.x + range * pMove.x, 
		                                            startObject.localPosition.y + range * pMove.y,
		                                              1);
		chess.currentPos.x = pos.x;
		chess.currentPos.y = pos.y;

		//Show if hidden
		if (chess.color == (int)ChessColor.BLUEHIDDEN || chess.color == (int)ChessColor.REDHIDDEN) {
			JsonObject param  = new JsonObject();
			param ["room"] = _table.roomID;
			JsonObject paramPos = new JsonObject ();
			paramPos ["x"] = chess.currentPos.x;
			paramPos ["y"] = chess.currentPos.y;
			param ["coord"] = paramPos;
			pomelo.request (Constants.SHOWHIDDENCHESS, param, this.OnQueryShowHiddenChess);
		}
	}

	void Captured (Point chessPos){
		ChessController chess =	FindChessWithPiece (chessPos, nameChessDestroy);
		if (nameChessDestroy == null) {
			chess = FindChessWithPiece (chessPos);
		} 
		else {
			nameChessDestroy = null;
		}
		creator.CreateChessCaptured(chess.color, nameChessCaptured);
		chess.currentPos = null;
		Destroy (chess.gameObject);
	}

	void Dangerous(Point dangerPos){
		Transform startObject = creator.startObject;
		float range = creator.range;
		if (_color == PlayerColor.kPlayerColorGuest) {
			dangerPos.x = 8 - dangerPos.x;
		}

		_dangerous.transform.localPosition = new Vector3(startObject.localPosition.x + range * dangerPos.x, 
		                                                 startObject.localPosition.y + range * dangerPos.y,
		                                            1);
		_dangerous.transform.localScale = Vector3.one;
	}

	void NotDangerous(){
		_dangerous.transform.localPosition = Vector3.zero;
		_dangerous.transform.localScale = Vector3.zero;
	}

	ChessController FindChessWithPiece(Point pos){
		foreach (var chess in _lsChess) {
			if (chess != null && chess.currentPos != null){
				if (chess.currentPos.x == pos.x && chess.currentPos.y == pos.y){
					return chess;
				}
			}
		}

		return null;
	}

	ChessController FindChessWithPiece(Point pos, string nameChess){
		foreach (var chess in _lsChess) {
			if (chess != null && chess.currentPos != null){
				if (chess.currentPos.x == pos.x && chess.currentPos.y == pos.y && chess._name.Equals(nameChess)){
					return chess;
				}
			}
		}
		
		return null;
	}

	void ShowHiddenChess(Point point, string newChessName){
		ChessController chess = FindChessWithPiece (point);
		creator.ShowHiddenChess (chess, newChessName);
	}

	public void ResetGame(){
		//Lam moi ban choi
		_isSelected = false;
		foreach (var obj in _lsCheckmated) {
			if (obj != null ) Destroy(obj.gameObject);
		}
		foreach (var obj in _lsChess) {
			if (obj.currentPos != null ) Destroy(obj.gameObject);
		}
		_lsCheckmated.Clear ();
		_lsChess.Clear ();
		
		isCreate = false; //create map
		isMove = false; //pomelo.on("onMoved")
		_createMate = false; //pomelo.on("onPath")
		_isCaptured = false; //pomelo.on("Capture")
		_isDangerous = false; //pomelo.on("onDangerous")
		_isNotDangerous = false; //pomelo.on("onNotDangerous")
		_hasMove = false; //Callback after move
		_isEndgame = false;
		gameState = GameplayState.kGameplayWaiting;
		btnStart.SetActive (true);
	}

	public void ShowOrHideTmpPanel(){
		isShowTmpPanel = !isShowTmpPanel;
		Vector3 targetPos = new Vector3 (0, 0, camera.transform.localPosition.z);
		if (isShowTmpPanel) {
			targetPos = new Vector3 (384, 0, camera.transform.localPosition.z);
		}
		GUITween.MoveTo( camera.gameObject , GUITween.Hash("position", targetPos, "islocal", true, "time", 0.5f, "delay", 0, "easeType", GUITween.EaseType.spring, "ignoretimescale" , false ));
	}

	public void ExitTable(){
		gameManager.PressedBack ();
	}
	
}
