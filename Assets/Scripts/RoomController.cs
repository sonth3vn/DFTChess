using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Pomelo.DotNetClient;
using SimpleJson;

public class RoomController : MonoBehaviour {
	PomeloClient pclient;
	
	public GameObject createPanel;
	private CreatePanelController createPanelTable;
	public Transform roomPanel;
	public Dictionary<string, TableController> lsTablePlay;
	public Dictionary<string, TableController> lsTableWait;
	public int _numTablePlay; //listen variable
	public IDictionary _newData;

	//sonth
	public List<IDictionary> lsDataPlay = new List<IDictionary>();
	public Dictionary<string, int> lsIdxTablePlay = new Dictionary<string, int> ();

	//public List<string> lsDeletePlayID = new List<string>();

	public bool isPlay = false;

	public InputField txtTableName;
	public InputField txtPassword;
	
	[HideInInspector] public int typeBoard;
	[HideInInspector] public int rubyBoard;
	[HideInInspector] public int timeBoard;

	[Header("Table")]
	public GameObject prefabTable;

	[Header("Panel switch")]
	public Transform playPanel;
	public Transform watchPanel;
	public Image imgBtnPlay;
	public Image imgBtnWatch;
	public bool isPlayPanel = true;
	public Transform startObject;
	public float rangeY = 115.0f;

	public Sprite spActive;
	public Sprite spDeactive;

	[Header("Table property")]
//	public Sprite[] lsAvatar;
	public Sprite[] lsType;

	public JsonObject _dataBoard;

	public bool f5PanelPlay = false;
	public bool f5PanelWait = false;
	
	// Use this for initialization
	void Start () {
		lsTablePlay = new Dictionary<string, TableController> ();
		lsTableWait = new Dictionary<string, TableController> ();
		pclient = GameManager._pclient;
		Util.loadPictureCB = LoadPictureCallback;
		createPanelTable = createPanel.GetComponent<CreatePanelController> ();

		LoadConfigCreateTable (); //load config create chess board

		Table oldTable = GameManager.instance.oldTable;
		txtTableName.text = oldTable.name;

		//Create table
		if (GameManager.instance.roomData.Length > 0) {
			IList iLsTable = Util.DeserializeJsonArrayToList(GameManager.instance.roomData);
			foreach (var obj in iLsTable){
				IDictionary dict = obj as IDictionary;
				lsDataPlay.Add(dict);
				f5PanelPlay = true;
				string id = System.Convert.ToString(dict["id"]);
				lsIdxTablePlay.Add(id, (lsDataPlay.Count - 1));
//				CreateNewTable(dict, true);
			}
			//_numTablePlay += iLsTable.Count;
		}

		//Handle events
		pclient.on("onNewRoom", (data) => {
			Debug.Log("onNewRoom");
			Debug.Log (data["room"]);
			//_newData = Util.DeserializeJsonToDict(System.Convert.ToString(data["room"]));
			//_numTablePlay += 1; // Thay doi numTable de loop trong update chay
			IDictionary roomDict = Util.DeserializeJsonToDict(System.Convert.ToString(data["room"]));
			lsDataPlay.Add(Util.DeserializeJsonToDict(System.Convert.ToString(data["room"])));
			string id = System.Convert.ToString(roomDict["id"]);
			lsIdxTablePlay.Add (id, (lsDataPlay.Count - 1));
			f5PanelPlay = true;
		});

		pclient.on("onDeleteRoom", (data) => {
			Debug.Log (data);
			string tableID = System.Convert.ToString(data["room"]);
			int idx = lsIdxTablePlay[tableID];
			lsDataPlay.RemoveAt(idx);
			lsIdxTablePlay.Remove(tableID);
			f5PanelPlay = true;
//			_numTablePlay -= 1;
//			lsDeletePlayID.Add(tableID);
		});

		GameManager.instance._rc = this;
		StartCoroutine (ListenRoomAction ());
	}

	IEnumerator ListenRoomAction(){
		while (!isPlay)
		{
			yield return 0; // wait for next frame
		}

		//parse data
		//set gia tri cho _table
		Table table = GameManager.instance._table;
		if (table == null) {
			// Khi player tao ban, table da duoc set 
			// Set cac gia tri cua table khi join ban
			table.ruby = System.Convert.ToInt32(_dataBoard["ruby"]);
			table.type = System.Convert.ToInt32(_dataBoard["type"]);
			table.time = System.Convert.ToInt32(_dataBoard["time"]);
			
			JsonObject jsonHost = (JsonObject)_dataBoard ["host"];
			Player host = table.host;
			host.name = System.Convert.ToString(jsonHost["name"]);
			host.uid = System.Convert.ToString(jsonHost["uid"]);
			host.sid = System.Convert.ToString(jsonHost["sid"]);
			host.roomName = System.Convert.ToString(jsonHost["rname"]);
			host.avatar = System.Convert.ToString(jsonHost["avatar"]);
			host.star = System.Convert.ToInt32(jsonHost["star"]);
			host.ruby = System.Convert.ToInt32(jsonHost["ruby"]);
			host.roomID = System.Convert.ToString(jsonHost["room"]);
			
			table.roomID = host.roomID;
			
			JsonObject jsonGuest = (JsonObject)_dataBoard ["guest"];
			if (jsonGuest.ContainsKey("uid")){
				// guest not null
				Player guest = table.guest;
				guest.name = System.Convert.ToString(jsonGuest["name"]);
				guest.uid = System.Convert.ToString(jsonGuest["uid"]);
				guest.sid = System.Convert.ToString(jsonGuest["sid"]);
				guest.roomName = System.Convert.ToString(jsonGuest["rname"]);
				guest.avatar = System.Convert.ToString(jsonGuest["avatar"]);
				guest.star = System.Convert.ToInt32(jsonGuest["star"]);
				guest.ruby = System.Convert.ToInt32(jsonGuest["ruby"]);
				guest.roomID = System.Convert.ToString(jsonGuest["room"]);
			}
		}
	
		GameManager.instance.ChangeState (GameState.kStateJoinTable);
		Application.LoadLevel("GameScene");
	}

	void Update(){
//		if (_numTablePlay > lsTablePlay.Keys.Count) {
//			CreateNewTable(_newData, true);
//		}

		if (f5PanelPlay) {
			f5PanelPlay = false;
			RefreshPanelPlay();
		}

//		if (lsDeletePlayID.Count > 0) {
//			for (int i = 0; i < lsDeletePlayID.Count; i++){
//				DeleteTable(lsDeletePlayID[i], true);
//				lsDeletePlayID.RemoveAt(i);
//			}
//		}
	}

	void RefreshPanelPlay(){
		foreach (var obj in lsTablePlay.Keys) {
			TableController table = lsTablePlay[obj];
			Destroy(table.gameObject);
		}
		lsTablePlay.Clear ();

		foreach (var data in lsDataPlay) {
			CreateNewTable(data, true);
		}
	}

	void CreateNewTable(IDictionary data, bool addTablePlay){
		//_numTablePlay ++; //Tang bien count
		Transform parentPanel = playPanel;
		int index = lsTablePlay.Keys.Count;
		string status = "Waiting ...";
		Dictionary<string, TableController> lsTable = lsTablePlay;
		TableType type = TableType.kTableTypePlay;
		if (!addTablePlay) {
			parentPanel = watchPanel;
			index = lsTableWait.Keys.Count;
			status = "Playing ...";
			lsTable = lsTableWait;
			type = TableType.kTableTypeWatch;
		}

		GameObject instObj = (GameObject)Instantiate (prefabTable, 
		                                              Vector3.zero,
		                                              Quaternion.identity);
		instObj.transform.SetParent(parentPanel);
		instObj.transform.localScale = Vector3.one;
		instObj.transform.localPosition = new Vector3(0, 
		                                              startObject.localPosition.y - index * rangeY,
		                                              1);
//		instObj.transform.localPosition = Vector3.zero;

		TableController tableController = instObj.GetComponent<TableController>();
		Table table = tableController.table;
		Player host = table.host;
		//Parse property
//		JsonObject roomJson = (JsonObject)data["room"];
		table.roomID = System.Convert.ToString (data["id"]);
		table.status = System.Convert.ToString (data["status"]);
		table.ruby = System.Convert.ToInt32 (data["ruby"]);
		table.type = System.Convert.ToInt32 (data["type"]);
		table.time = System.Convert.ToInt32 (data["time"]);

		IDictionary hostJson = data ["host"] as IDictionary;
		host.name = System.Convert.ToString (hostJson["name"]);
		host.uid = System.Convert.ToString (hostJson["uid"]);
		host.sid = System.Convert.ToString (hostJson["sid"]);
		host.roomName = System.Convert.ToString (hostJson["rname"]);
		host.avatar = System.Convert.ToString (hostJson["avatar"]);
		host.star = System.Convert.ToInt32 (hostJson["star"]);
		host.ruby = System.Convert.ToInt32 (hostJson["ruby"]);
		host.roomID = System.Convert.ToString (hostJson["room"]);

		//Setup Table label & image
		tableController.lblIndex.text = "" + (index + 1);
		string spriteURL = Constants.DOMAIN + host.avatar;
		Util util = Util.instance;
		util.LoadPictureFromURL (spriteURL, table.roomID);
//		tableController.spAvatar.sprite = lsAvatar[int.Parse (host.avatar)];
		tableController.lblLevel.text = "" + 1; //fake
		tableController.lblUsername.text = host.name;
		tableController.lblCoin.text = "" + table.ruby;
		tableController.lblTime.text = "" + (table.time / 60) + "\'";
		tableController.lblStatus.text = table.status;
		tableController.spType.sprite = lsType [table.type];
		tableController.type = type;

		//Them vao list, voi key = id
		lsTable.Add (table.roomID, tableController);

	}

	void DeleteTable(string tableID, bool deleteTablePlay){
		Dictionary<string, TableController> lsTable = lsTablePlay;
		if (!deleteTablePlay) {
			lsTable = lsTableWait;
		}
		TableController table = lsTable [tableID];
		if (table != null && lsTable.ContainsKey(tableID)) {
			lsTable.Remove (tableID);
			Destroy (table.gameObject);
		}
	}

	public void PressedSwitchPanel(){
		isPlayPanel = !isPlayPanel;
		if (isPlayPanel) {
			imgBtnPlay.sprite = spActive;
			imgBtnWatch.sprite = spDeactive;
			playPanel.localScale = Vector3.one;
			watchPanel.localScale = Vector3.zero;
		}
		else{
			imgBtnPlay.sprite = spDeactive;
			imgBtnWatch.sprite = spActive;
			watchPanel.localScale = Vector3.one;
			playPanel.localScale = Vector3.zero;
		}
	}

	void LoadPictureCallback(Texture2D result, string tableID){
		if (result == null) {
			return;
		}

		TableController table = lsTablePlay [tableID];
		table.spAvatar.sprite = Sprite.Create (result, table.spAvatar.sprite.rect, new Vector2(0.5f, 0.5f));
		//source.name = "123";
	}

	public void PressedCreateTable(){
		createPanel.SetActive (true);
	}

	public void CreateNewTable(){
		// Type, ruby, time nhan duoc tu bang selection 
		// Doc method ChooseItem trong CreatePanelController
		// Fake table name
		string tableName = txtTableName.text;
		string password = txtPassword.text;
		int type = typeBoard;
		int ruby = rubyBoard;
		int time = timeBoard;
		Table table = new Table ();
		table.name = tableName;
		table.type = type;
		table.ruby = ruby;
		table.time = time;
		table.password = password;
		GameManager.instance.oldTable = table; //Luu lai de dung cho lan tao ban tiep theo
		GameManager.instance.CreateTable (table);
	}

	public void CancelCreateTable(){
		createPanel.SetActive (false);
	}

	public void LoadConfigCreateTable(){
		string dataBoard = GameManager.instance.dataCreateTable;
		//Fake
		for (int i = 0; i < 4; i++) {
			string strCap = "Cờ tướng thường";
			if (i == 1){
				strCap = "Cờ tướng úp";
			}
			else if (i == 2){
				strCap = "Cờ úp sĩ chúa";
			}
			else if (i == 3){
				strCap = "Cờ chấp nước";
			}
			CellCreateTableSelection cell = new CellCreateTableSelection{
				caption = strCap,
				value = i
			};
			createPanelTable.AddItemTypeChess(cell);
		}

		for (int i = 0; i < 4; i++) {
			string strCap = "1000";
			if (i == 1){
				strCap = "5000";
			}
			else if (i == 2){
				strCap = "10000";
			}
			else if (i == 3){
				strCap = "20000";
			}
			CellCreateTableSelection cell = new CellCreateTableSelection{
				caption = strCap,
				value = int.Parse(strCap)
			};
			createPanelTable.AddItemCoinBet(cell);
		}

		for (int i = 0; i < 4; i++) {
			string strCap = "300";
			if (i == 1){
				strCap = "600";
			}
			else if (i == 2){
				strCap = "1200";
			}
			else if (i == 3){
				strCap = "3600";
			}
			CellCreateTableSelection cell = new CellCreateTableSelection{
				caption = "" + (int.Parse(strCap) / 60) + " phút",
				value = int.Parse(strCap)
			};
			createPanelTable.AddItemTimePlay(cell);
		}

		createPanelTable.RefreshAllDropDownList ();
	}
	
	
}
