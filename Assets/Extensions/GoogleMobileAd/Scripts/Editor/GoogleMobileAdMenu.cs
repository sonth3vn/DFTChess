////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

public class GoogleMobileAdMenu : EditorWindow {
	


	#if UNITY_EDITOR

	//--------------------------------------
	//  GENERAL
	//--------------------------------------

	[MenuItem("Window/GoogleMobileAd/Edit Settings")]
	public static void Edit() {
		Selection.activeObject = GoogleMobileAdSettings.Instance;
	}
	

	#endif

}
