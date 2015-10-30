using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJson;

public class Player{
	public string name { get; set; }
	public string uid { get; set; }
	public string sid { get; set; }
	public string roomName { get; set; }
	public string avatar { get; set; }
	public int star { get; set; }
	public int ruby { get; set; }
	public string roomID { get; set; }
}

public class Table{
	public string roomID { get; set; }
	public string name { get; set; }
	public Player host = new Player ();
	public Player guest = new Player ();
	public string status { get; set; }
	public int ruby { get; set; }
	public int type { get; set; }
	public int time { get; set; }
	public string password { get; set; }
}
