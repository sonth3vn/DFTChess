using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GUIMainCamera : MonoBehaviour {

	public GameObject pauseMenu ;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonDismissExample(){
		EventSystem.current.currentSelectedGameObject.GetComponent<GUIEffects>().DismissObjects();
	}
	
	public void ReplayScene(){
		Application.LoadLevel( Application.loadedLevel );
	}

	/*
	public void PauseMenuExample(){
		GameObject menu = GameObject.Instantiate( pauseMenu, new Vector3(0,0,0), Quaternion.identity ) as GameObject ;
		menu.transform.SetParent( GameObject.Find("Canvas").transform );
		menu.transform.localPosition = new Vector3( 0,0,0 );
	}
	*/
}
