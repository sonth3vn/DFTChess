using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {

	public string _name;

	public void PressedJoin(){
		// Join vao ban choi
		GameManager.instance.PressedJoinServer (this);
	}
}
