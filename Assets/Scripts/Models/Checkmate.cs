using UnityEngine;
using System.Collections;

public class Checkmate : MonoBehaviour {
	public Point currentPos = new Point();

//	void Start(){
//		Rect rect = GetComponent<RectTransform> ().rect;
//		rect = new Rect (rect.position.x, rect.position.y, 100, 100);
//	}

	public void Pressed(){
		GameController gc = GameObject.FindObjectOfType<GameController> ();
		gc.MoveToPath (currentPos);
	}
}
