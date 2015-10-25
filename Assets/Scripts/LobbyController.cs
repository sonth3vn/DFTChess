using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		GameManager.instance.ChangeState (GameState.kStateJoinRoom);
		Application.LoadLevel("RoomScene");
	}
}
