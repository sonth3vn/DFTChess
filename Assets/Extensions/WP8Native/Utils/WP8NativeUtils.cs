using UnityEngine;
using System.Collections;

public class WP8NativeUtils  {


	public static void ShowPreloader() {
		WP8PopUps.PopUp.ShowPreLoader(100);
	}

	public static void HidePreloader() {
		WP8PopUps.PopUp.HidePreLoader();
	}

}
