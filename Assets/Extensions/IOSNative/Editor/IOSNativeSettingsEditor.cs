
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(IOSNativeSettings))]
public class IOSNativeSettingsEditor : Editor {

	public static bool ShowStoreKitParams = false;


	GUIContent AppleIdLabel = new GUIContent("Apple Id [?]:", "Your Application Apple ID.");
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");

	GUIContent SKPVDLabel = new GUIContent("Store Products View [?]:", "YThe SKStoreProductViewController class makes it possible to integrate purchasing from Apple’s iTunes, App and iBooks stores directly into iOS 6 applications with minimal coding work.");
	GUIContent CheckInternetLabel = new GUIContent("Check Internet Connection[?]:", "If set to true Internet connection will be checked before sending load request. Request will be sent automatically if network became available");


	private IOSNativeSettings settings;

	public override void OnInspectorGUI() {
		settings = target as IOSNativeSettings;

		GUI.changed = false;



		GeneralOptions();
		BillingSettings();


		AboutGUI();
	

		if(GUI.changed) {
			DirtyEditor();
		}
	}




	private void GeneralOptions() {


		EditorGUILayout.HelpBox("(Required) Application Data", MessageType.None);

		if (settings.AppleId.Length == 0 || settings.AppleId.Equals("XXXXXXXXX")) {
			EditorGUILayout.HelpBox("Invalid Apple Id", MessageType.Error);
		}



		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(AppleIdLabel);
		settings.AppleId	 	= EditorGUILayout.TextField(settings.AppleId);
		EditorGUILayout.EndHorizontal();




		EditorGUILayout.Space();

	}
	

	private void BillingSettings() {
		EditorGUILayout.HelpBox("(Optional) Edit the billing parameters", MessageType.None);
		ShowStoreKitParams = EditorGUILayout.Foldout(ShowStoreKitParams, "Billing Settings");
		if(ShowStoreKitParams) {

			if(settings.InAppProducts.Count == 0) {
				EditorGUILayout.HelpBox("No In-App Products Added", MessageType.Warning);
			}
		

			int i = 0;
			foreach(string str in settings.InAppProducts) {
				EditorGUILayout.BeginHorizontal();
				settings.InAppProducts[i]	 	= EditorGUILayout.TextField(settings.InAppProducts[i]);
				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.InAppProducts.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.InAppProducts.Add("");
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(CheckInternetLabel);
			settings.checkInternetBeforeLoadRequestl = EditorGUILayout.Toggle(settings.checkInternetBeforeLoadRequestl);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(SKPVDLabel);

			/*****************************************/

			if(settings.DefaultStoreProductsView.Count == 0) {
				EditorGUILayout.HelpBox("No Default Store Products View Added", MessageType.Info);
			}
			
			
			i = 0;
			foreach(string str in settings.DefaultStoreProductsView) {
				EditorGUILayout.BeginHorizontal();
				settings.DefaultStoreProductsView[i]	 	= EditorGUILayout.TextField(settings.DefaultStoreProductsView[i]);
				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.DefaultStoreProductsView.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.DefaultStoreProductsView.Add("");
			}
			EditorGUILayout.EndHorizontal();



			EditorGUILayout.Space();

		}
	}




	private void AboutGUI() {




		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, IOSNativeSettings.VERSION_NUMBER);
		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
		
		
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}



	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(IOSNativeSettings.Instance);
		#endif
	}
	
	
}
