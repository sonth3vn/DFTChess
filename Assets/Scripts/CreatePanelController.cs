using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;

public class CreatePanelController : MonoBehaviour {
	public RoomController rc;
	// Mang thong tin cac cell cau hinh tu server
	public List<CellCreateTableSelection> lsCellCoinBet = new List<CellCreateTableSelection>();
	public List<CellCreateTableSelection> lsCellTimePlay = new List<CellCreateTableSelection>();
	public List<CellCreateTableSelection> lsCellTypeChess = new List<CellCreateTableSelection>();

	// Dropdown list
	public DropDownList ddlCoinBet;
	public DropDownList ddlTimePlay;
	public DropDownList ddlTypeChess;

	// Sprite
	public Sprite imgCoin;

	// Use this for initialization
	void Start () {

	}

	public void AddItemCoinBet(CellCreateTableSelection item){
		lsCellCoinBet.Add (item);
	}

	public void AddItemTimePlay(CellCreateTableSelection item){
		lsCellTimePlay.Add (item);
	}

	public void AddItemTypeChess(CellCreateTableSelection item){
		lsCellTypeChess.Add (item);
	}
	
	public void RefreshAllDropDownList(){
		for (int i = 0; i < lsCellCoinBet.Count; i++) {
			CellCreateTableSelection cell = lsCellCoinBet[i];
			DropDownListItem item = new DropDownListItem(cell.caption, "COIN", cell.value, imgCoin, false, null);
			item.OnSelect = ChooseItem;
			ddlCoinBet.Items.Add(item);
		}

		for (int i = 0; i < lsCellTimePlay.Count; i++) {
			CellCreateTableSelection cell = lsCellTimePlay[i];
			DropDownListItem item = new DropDownListItem(cell.caption, "TIME", cell.value, imgCoin, false, null);
			item.OnSelect = ChooseItem;
			ddlTimePlay.Items.Add(item);
		}

		for (int i = 0; i < lsCellTypeChess.Count; i++) {
			CellCreateTableSelection cell = lsCellTypeChess[i];
			DropDownListItem item = new DropDownListItem(cell.caption, "TYPE", cell.value, imgCoin, false, null);
			item.OnSelect = ChooseItem;
			ddlTypeChess.Items.Add(item);
		}

		ddlCoinBet.Refresh ();
		ddlTimePlay.Refresh ();
		ddlTypeChess.Refresh ();

		SetupAllDropDownList ();
	}

	void ChooseItem(string id, int value){
		if (id.Equals ("COIN")) {
			rc.rubyBoard = value;
		}
		else if (id.Equals ("TIME")){
			rc.timeBoard = value;
		}
		else if (id.Equals ("TYPE")){
			rc.typeBoard = value;
		}
	}

	void SetupAllDropDownList(){
		Table oldTable = GameManager.instance.oldTable;
		int oldValueRuby = oldTable.ruby;
		int oldValueTime = oldTable.time;
		int oldValueType = oldTable.type;

		for (int i = 0; i < ddlCoinBet.Items.Count; i++) {
			DropDownListItem item = ddlCoinBet.Items[i];
			if (item.Value == oldValueRuby){
				ddlCoinBet.CloneOnItemClicked(i);
				break;
			}
		}

		for (int i = 0; i < ddlTimePlay.Items.Count; i++) {
			DropDownListItem item = ddlTimePlay.Items[i];
			if (item.Value == oldValueTime){
				ddlTimePlay.CloneOnItemClicked(i);
				break;
			}
		}

		for (int i = 0; i < ddlTypeChess.Items.Count; i++) {
			DropDownListItem item = ddlTypeChess.Items[i];
			if (item.Value == oldValueType){
				ddlTypeChess.CloneOnItemClicked(i);
				break;
			}
		}

	}
}
