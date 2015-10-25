using UnityEngine;
using System.Collections;
using SimpleJson;

public class Users {
	public string userID { get; set; }
	public string name { get; set; }
//	public string password;
	public string email { get; set; }
	public string avatar { get; set; }
	public string ruby { get; set; }
	public int vip { get; set; }
	public int exp { get; set; }
	public int star { get; set; }
	public JsonObject status { get; set; }
}
