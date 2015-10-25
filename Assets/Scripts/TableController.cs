using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TableType{
	kTableTypePlay,
	kTableTypeWatch
};

public class TableController : MonoBehaviour {
	public Table table = new Table ();
	[Header("Property")]
	public Text lblIndex;
	public Image spAvatar;
	public Text lblLevel;
	public Text lblUsername;
	public Text lblCoin;
	public Text lblTime;
	TableStatus _status;
	public Text lblStatus;
	public Image spType;

	public TableType type;

	// Use this for initialization
	void Start () {
		_status = TableStatus.kStatusNone;
	}

	public void UpdateStatus(TableStatus status){
		// Cap nhat status ban choi
		// Trong, dang doi, full
		_status = status;
		if (status == TableStatus.kStatusNone) {
			lblStatus.text = @"Trống";
		}
		else if (status == TableStatus.kStatusWait) {
			lblStatus.text = @"Đang đợi";
		}
		else if (status == TableStatus.kStatusFull) {
			lblStatus.text = @"Đầy";
		}
		else{
			Debug.LogError("Cap nhat sai gia tri status");
		}
	}

	public void PressedJoin(){
		GameManager.instance.PressedJoinTable (this);
	}

}
