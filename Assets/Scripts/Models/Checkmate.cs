using UnityEngine;
using System.Collections;

public class Checkmate : MonoBehaviour {
	public Point currentPos = new Point();
	public int X, Y;

	void Update(){
		X = currentPos.x;
		Y = currentPos.y;
	}

	public void Pressed(){
		GameController gc = GameObject.FindObjectOfType<GameController> ();
		gc.MoveToPath (currentPos);
	}
}
