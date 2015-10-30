using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Pomelo.DotNetClient;

public class ChatRecord
{
	public string name {get;set;}
	public string msg {get; set;}
	public string avatar {get; set;}
	
	public ChatRecord(){
		
	}
	//Chat record
	public ChatRecord (string avatar, string userName, string msg)
	{
		this.avatar = avatar;
		this.name = userName;
		this.msg = msg;
	}
}

public class ChatController : MonoBehaviour {
	PomeloClient pomelo;
	public InputField txtChatInput;
	public Transform startObject;
	public List<ChatRecord> lsRecord = new List<ChatRecord>();
	public List<GameObject> lsRecordUI = new List<GameObject>();
	public GameObject prefChatGUI;
	string username;
	float range = 170.0f;

	ChatRecord _record;
	bool _isLoad = false;

	// Use this for initialization
	void Start () {
		username = GameManager.instance._user.name;
		Util.loadPictureCB = LoadPictureCallback;
	}
	
	// Update is called once per frame
	void Update () {
		if (_isLoad) {
			_isLoad = false;
			AddUI();
		}
	}

	public void EnterChat(){
		string msg = txtChatInput.text;
		GameManager.instance.EnterChat (msg);
		txtChatInput.text = "";
	}

	public void HandlerChat(ChatRecord record){
		_record = record;
		_isLoad = true;
	}

	public void AddUI(){
		if (lsRecord.Count >= 5) {
			lsRecord.Clear();
		}
		lsRecord.Add (_record);
		RefreshView ();
	}

	public void RefreshView(){
		RemoveAllObjFromParent (lsRecordUI);
		for (int i = 0; i < lsRecord.Count; i++) {
			GameObject instObj = (GameObject)Instantiate (prefChatGUI, 
			                                              Vector3.zero,
			                                              Quaternion.identity);
			instObj.transform.SetParent(transform);
			instObj.transform.localPosition = new Vector3(startObject.localPosition.x, 
			                                              startObject.localPosition.y - range * i,
			                                              1);
			instObj.transform.localScale = Vector3.one;
			lsRecordUI.Add(instObj);
			ChatRecordUI record = instObj.GetComponent<ChatRecordUI>();
			record.lblMsg.text = lsRecord[i].msg;
			record.lblUsername.text = lsRecord[i].name;
			Util util = Util.instance;
			util.LoadPictureFromURL (Constants.DOMAIN + lsRecord[i].avatar, "" + (lsRecordUI.Count - 1));

//			record.imgAvatar.sprite = lsRecord[i].msg;
		}
	}

	void LoadPictureCallback(Texture2D result, string recordIdx){
		if (result == null) {
			return;
		}
		
		GameObject obj = lsRecordUI [int.Parse (recordIdx)];
		ChatRecordUI record = obj.GetComponent<ChatRecordUI>();
		record.imgAvatar.sprite = Sprite.Create (result, record.imgAvatar.sprite.rect, new Vector2(0.5f, 0.5f));
		//source.name = "123";
	}

	public void CloseChat(){
		gameObject.SetActive (false);
	}

	public void RemoveAllObjFromParent(List<GameObject> list){
		for (int i = 0; i < list.Count; i++) {
			Destroy(list[i].gameObject);
		}
		lsRecordUI.Clear ();
	}

}
