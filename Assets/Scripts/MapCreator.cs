// new stuff
using System.Linq;
using System;
//using UnityEditor;
// standard
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class Point{
	public int x{ get; set;}
	public int y{ get; set;}
}

public class MapCreator : MonoBehaviour {
	//public string imagePath = "Assets/Sprites/Game/Chess/001/";
	public Sprite spBlueChess;
	public Sprite spRedChess;

	public Dictionary<string, Sprite> lsImageBlue;
	public Dictionary<string, Sprite> lsImageRed;

	[Header("Reference Object")]
	public Transform chessPanel;
	public Transform startObject;
	public GameObject prefChess;
	public float range;
	public PlayerColor playerColor;

	public GameObject prefCheckmate;

	//Share from game controller
	List<GameObject> lsCheckmate;
	List<ChessController> lsChess;

	[Header("Resource")]
	public Sprite[] lsSpriteBlue;
	public Sprite[] lsSpriteRed;

	[Header("Temp Panel")]
	public Transform startPCaptured;
	public Transform startOtherCaptured;
	public Transform pCapturedPanel;
	public Transform otherCapturedPanel;
	int iPCaptured, iOtherCaptured;

	// Use this for initialization
	void Start () {
//		Sprite[] lsSpriteBlue = AssetDatabase.LoadAllAssetRepresentationsAtPath (AssetDatabase.GetAssetPath (spBlueChess)).OfType<Sprite>().ToArray();
//		Sprite[] lsSpriteRed = AssetDatabase.LoadAllAssetRepresentationsAtPath (AssetDatabase.GetAssetPath (spRedChess)).OfType<Sprite>().ToArray();
		lsImageBlue = new Dictionary<string, Sprite> ();
		lsImageRed = new Dictionary<string, Sprite> ();
		foreach (var obj in lsSpriteBlue) {
			lsImageBlue.Add(obj.name, obj);
		}
		foreach (var obj in lsSpriteRed) {
			lsImageRed.Add(obj.name, obj);
		}

		GameController gc = gameObject.GetComponent<GameController> ();
		lsChess = gc._lsChess;
		lsCheckmate = gc._lsCheckmated;
	}

	public void CreateMap(string dataBoard){
		IList lsData = Util.DeserializeJsonArrayToList (dataBoard);
		foreach (var obj in lsData) {
			IDictionary chessData = obj as IDictionary;
			string chessName = System.Convert.ToString(chessData["name"]);
			IDictionary dictPos = chessData["pos"] as IDictionary;
			int posX = System.Convert.ToInt32(dictPos["x"]);
			int posY = System.Convert.ToInt32(dictPos["y"]);
			Point pos = new Point{x = posX, y = posY};
			int color = System.Convert.ToInt32(chessData["color"]);
			CreateChess(chessName, pos, color);
		}
	}

	public void CreateChess(string chessName, Point pos, int color){
		// Create va set vi tri object
		Point pCreate = new Point{x = pos.x, y = pos.y};
		if (playerColor == PlayerColor.kPlayerColorGuest) {
			pCreate.x = 8 - pCreate.x;
		}
		else{
			pCreate.y = 9 - pCreate.y;
		}

		GameObject instObj = (GameObject)Instantiate (prefChess, 
		                                              Vector3.zero,
		                                              Quaternion.identity);
		instObj.transform.SetParent(chessPanel);
		instObj.transform.localScale = new Vector3(0.8f, 0.8f, 1);
		instObj.transform.localPosition = new Vector3(startObject.localPosition.x + range * pCreate.x, 
		                                              startObject.localPosition.y + range * pCreate.y,
		                                              1);

		if ((int)GameController.instance._color % 10 != -color % 10) {
			Destroy(instObj.GetComponent<Button>());
		}

		// Thay doi image source
		Dictionary<string, Sprite> lsImageSource = lsImageBlue;
		if (color == (int)ChessColor.RED || color == (int)ChessColor.REDHIDDEN) {
			lsImageSource = lsImageRed;
		}
		Image image = instObj.GetComponent<Image> ();
		image.sprite = lsImageSource [chessName];

		// Set thuoc tinh cho chess
		ChessController chessController = instObj.GetComponent<ChessController> ();
		chessController.currentPos = pos;
		chessController.color = color;
		chessController._name = chessName;

		lsChess.Add (chessController);
	}

	public void CreateCheckMate(Point pos){
		// Create va set vi tri object
		Point pCreate = new Point{x = pos.x, y = pos.y};
		if (playerColor == PlayerColor.kPlayerColorGuest) {
			pCreate.x = 8 - pCreate.x;
			pCreate.y = 9 - pCreate.y;
			pos.y = 9 - pos.y;
		}
		else{
			pCreate.y = 9 - pCreate.y;
		}
		GameObject instObj = (GameObject)Instantiate (prefCheckmate, 
		                                              Vector3.zero,
		                                              Quaternion.identity);
		instObj.transform.SetParent(chessPanel);
		instObj.transform.localScale = new Vector3 (0.75f, 0.75f, 1);
		instObj.transform.localPosition = new Vector3(startObject.localPosition.x + range * pCreate.x, 
		                                              startObject.localPosition.y + range * pCreate.y,
		                                              1);
//		pos.y = 9 - pos.y;
//		if (playerColor == PlayerColor.kPlayerColorGuest) {
//			instObj.transform.localPosition = new Vector3(startObject.localPosition.x + range * (8 - pos.x), 
//			                                              startObject.localPosition.y + range * (9 - pos.y),
//			                                              1);
////			pos.y = 9 - pos.y;
//		}

		lsCheckmate.Add (instObj);

		Checkmate checkmate = instObj.GetComponent<Checkmate> ();
		checkmate.currentPos = pos;
	}

	public void ShowHiddenChess(ChessController chess, string newName){
		Dictionary<string, Sprite> lsImageSource;
		if (chess.color == (int)ChessColor.BLUEHIDDEN) {
			lsImageSource = lsImageBlue;
		}
		else{
			lsImageSource = lsImageRed;
		}
		chess.GetComponent<Image> ().sprite = lsImageSource [newName];
		chess._name = newName;
		chess.color = chess.color % 10;
	}

	public void CreateChessCaptured(int color, string chessName){
		// Create va set vi tri object
		GameObject instObj = (GameObject)Instantiate (prefChess, 
		                                              Vector3.zero,
		                                              Quaternion.identity);
		Transform parentPanel = pCapturedPanel;
		Transform startObj = startPCaptured;
		int count = iPCaptured;
		Dictionary<string, Sprite> lsImageSource = lsImageBlue;
		if (color == (int)ChessColor.RED || color == (int)ChessColor.REDHIDDEN) {
			lsImageSource = lsImageRed;
		}
		if (color % 10 != (int)playerColor) {
			parentPanel = otherCapturedPanel;
			startObj = startOtherCaptured;
			count = iOtherCaptured;
			iOtherCaptured++;
		}
		else{
			iPCaptured++;
		}
		instObj.transform.SetParent(parentPanel);
		instObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (50, 50);
		instObj.transform.localPosition = new Vector3(startObj.localPosition.x + 60 * (count % 5), 
		                                              startObj.localPosition.y - 60 * (count / 5),
		                                              1); //Mot hang chua toi da 5 quan
		instObj.transform.localScale = Vector3.one;
		Image image = instObj.GetComponent<Image> ();
		image.sprite = lsImageSource [chessName];
		Destroy(instObj.transform.GetComponent<ChessController>());
		Destroy(instObj.transform.GetComponent<Button>());

	}
}
