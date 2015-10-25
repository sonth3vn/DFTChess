using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Pomelo.DotNetClient;
using SimpleJson;

public class RoomController : MonoBehaviour {
	PomeloClient pclient;

	public Transform roomPanel;
	public Dictionary<string, TableController> lsTablePlay;
	public Dictionary<string, TableController> lsTableWait;
	public int _numTablePlay; //listen variable
//	public int _counting;
	public IDictionary _newData;
	public List<string> lsDeletePlayID = new List<string>();

	public bool isPlay = false;

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

	JsonObject _dataOnStatus;

	// Use this for initialization
	void Start () {
		lsTablePlay = new Dictionary<string, TableController> ();
		lsTableWait = new Dictionary<string, TableController> ();
		pclient = GameManager._pclient;
		Util.loadPictureCB = LoadPictureCallback;

		//Create table
		if (GameManager.instance.roomData.Length > 0) {
			IList iLsTable = Util.DeserializeJsonArrayToList(GameManager.instance.roomData);
			foreach (var obj in iLsTable){
				IDictionary dict = obj as IDictionary;
				//string id = System.Convert.ToString(dict["id"]);
//				_newData = dict["id"];
//				_numTablePlay += 1;
				CreateNewTable(dict, true);
			}
			_numTablePlay += iLsTable.Count;
		}

		//Handle events
		pclient.on("onNewRoom", (data) => {
			Debug.Log (data["room"]);
			_newData = Util.DeserializeJsonToDict(System.Convert.ToString(data["room"]));
			_numTablePlay += 1; // Thay doi numTable de loop trong update chay
		});

		pclient.on("onDeleteRoom", (data) => {
			Debug.Log (data);
			string tableID = System.Convert.ToString(data["room"]);
			_numTablePlay -= 1;
			lsDeletePlayID.Add(tableID);
		});

		GameManager.instance._rc = this;
		StartCoroutine (ListenRoomAction ());
	}

	IEnumerator ListenRoomAction(){
		while (!isPlay)
		{
			yield return 0; // wait for next frame
		}
		
		GameManager.instance.ChangeState (GameState.kStateJoinTable);
		Application.LoadLevel("GameScene");
	}

	void Update(){
		if (_numTablePlay > lsTablePlay.Keys.Count) {
			CreateNewTable(_newData, true);
		}

		if (lsDeletePlayID.Count > 0) {
			for (int i = 0; i < lsDeletePlayID.Count; i++){
				DeleteTable(lsDeletePlayID[i], true);
				lsDeletePlayID.RemoveAt(i);
			}
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
		if (table != null) {
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
	
	
}
