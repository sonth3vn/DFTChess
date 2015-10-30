using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class LobbyController : MonoBehaviour {

	public List<TableController> lsTable;

	public bool isJoin = false;

	// Use this for initialization
	void Start () {
		GameManager.instance._lobby = this;
		StartCoroutine (ListenLobbyAction ());
	}
	
	IEnumerator ListenLobbyAction(){
		while (!isJoin)
		{
			yield return 0; // wait for next frame
		}
		Users user = GameManager.instance._user;
		JsonObject currentUser = GameManager.instance._currentUser;
		user.userID = System.Convert.ToString(currentUser["_id"]); 
		user.name = System.Convert.ToString(currentUser["name"]);
		user.star = System.Convert.ToInt32(currentUser["star"]);
		user.exp = System.Convert.ToInt32(currentUser["exp"]);
		user.vip = System.Convert.ToInt32(currentUser["vip"]);
		user.ruby = System.Convert.ToString(currentUser["ruby"]);
		user.avatar = System.Convert.ToString(currentUser["avatar"]);
		user.email = System.Convert.ToString(currentUser["email"]);
		GameManager.instance.ChangeState (GameState.kStateJoinRoom);
		Application.LoadLevel("RoomScene");
	}
}
