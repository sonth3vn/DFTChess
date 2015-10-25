using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChessController : MonoBehaviour {
	GameController gc;

	[Header("Property")]
	public Point currentPos = new Point ();
	public int color;
	public string _name;
	public int X, Y;

	// Use this for initialization
	void Start () {
		gc = GameController.instance;
	}
	
	// Update is called once per frame
	void Update () {
		X = currentPos.x;
		Y = currentPos.y;
	}

	/* --- Helper method --- */
	public void PressedSelect(){
		gc._chessSelected = this;
		gc.SelectChess (color % 10, currentPos);
	}
	
}
