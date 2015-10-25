using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum EndgameType{
	kEndgameTypeWin,
	kEndgameTypeLose,
	kEndgameEndTime,
	kEndgamePlayerExit,
};

public class EndgameController : MonoBehaviour {
	public Image imgWin;
	public Image imgLose;
	public Image imgEndtime;
	public GameController gc;
	public void Endgame(EndgameType type){
		gameObject.SetActive (true);
		if (type == EndgameType.kEndgameTypeWin){
			imgWin.gameObject.SetActive(true);
		}
		else if (type == EndgameType.kEndgameTypeLose){
			imgLose.gameObject.SetActive(true);
		}
		else{
			imgEndtime.gameObject.SetActive(true);
		}
	}

	public void Reset(){
		gameObject.SetActive (false);
		imgWin.gameObject.SetActive (false);
		imgLose.gameObject.SetActive (false);
		imgEndtime.gameObject.SetActive (false);
		gc.ResetGame ();
	}
	

}
