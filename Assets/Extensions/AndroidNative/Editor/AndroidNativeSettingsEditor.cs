
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(AndroidNativeSettings))]
public class AndroidNativeSettingsEditor : Editor {



	
	
	GUIContent GamesApiLabel   = new GUIContent("Enable Games API [?]:", "API used for achivements and leaderboards");
	GUIContent AppSateApiLabel = new GUIContent("Enable App State API [?]:", "API used for cloud data save");


	GUIContent Base64KeyLabel = new GUIContent("Base64 Key[?]:", "Base64 Key app key.");
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");


	private AndroidNativeSettings settings;


	private const string version_info_file = "Plugins/StansAssets/Versions/AN_VersionInfo.txt"; 

	public override void OnInspectorGUI() {
		settings = target as AndroidNativeSettings;

		GUI.changed = false;



		GeneralOptions();

		PlayServiceSettings();

		EditorGUILayout.Space();
		BillingSettings();

		EditorGUILayout.Space();
		SocialPlatfromSettingsEditor.FacebookSettings();
		EditorGUILayout.Space();
		SocialPlatfromSettingsEditor.TwitterSettings();
		EditorGUILayout.Space();


		AboutGUI();
	

		if(GUI.changed) {
			DirtyEditor();
		}

	}

	public static bool IsInstalled {
		get {
			if(FileStaticAPI.IsFileExists(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "androidnative.jar")) {
				return true;
			} else {
				return false;
			}
		}
	}

	public static bool IsUpToDate {
		get {
			if(AndroidNativeSettings.VERSION_NUMBER.Equals(DataVersion)) {
				return true;
			} else {
				return false;
			}
		}
	}


	public static string DataVersion {
		get {
			if(FileStaticAPI.IsFileExists(version_info_file)) {
				return FileStaticAPI.Read(version_info_file);
			} else {
				return "Unknown";
			}
		}
	}

	public static void UpdateVersionInfo() {
		FileStaticAPI.Write(version_info_file, AndroidNativeSettings.VERSION_NUMBER);
	}

	private void GeneralOptions() {



		if(!IsInstalled) {
			EditorGUILayout.HelpBox("Install Required ", MessageType.Error);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			Color c = GUI.color;
			GUI.color = Color.cyan;
			if(GUILayout.Button("Install Plugin",  GUILayout.Width(120))) {
				PluginsInstalationUtil.Android_InstallPlugin();
				UpdateVersionInfo();
			}
			GUI.color = c;
			EditorGUILayout.EndHorizontal();
		}

		if(IsInstalled) {
			if(!IsUpToDate) {
				EditorGUILayout.HelpBox("Update Required \nResources version: " + DataVersion + " Plugin version: " + AndroidNativeSettings.VERSION_NUMBER, MessageType.Warning);
			

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				Color c = GUI.color;
				GUI.color = Color.cyan;
				if(GUILayout.Button("Update to " + AndroidNativeSettings.VERSION_NUMBER,  GUILayout.Width(250))) {
					PluginsInstalationUtil.Android_UpdatePlugin();
					UpdateVersionInfo();
				}

				GUI.color = c;
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();

			} else {
				EditorGUILayout.HelpBox("Android Native Plugin v" + AndroidNativeSettings.VERSION_NUMBER + " is installed", MessageType.Info);
				Actions();
			}
		}


		EditorGUILayout.Space();

	}

	private void Actions() {
		EditorGUILayout.Space();
		AndroidNativeSettings.Instance.ShowActions = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowActions, "More Actions");
		if(AndroidNativeSettings.Instance.ShowActions) {

			if(!FileStaticAPI.IsFolderExists("Facebook")) {
				GUI.enabled = false;
			}	

			if(GUILayout.Button("Remove Facebook SDK",  GUILayout.Width(160))) {

			
				bool result = EditorUtility.DisplayDialog(
					"Removing Facebook SDK",
					"Warning action can not be undone without reimporting the plugin",
					"Remove",
					"Cansel");

				if(result) {
					FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "facebook");
					FileStaticAPI.DeleteFolder("Facebook");
					FileStaticAPI.DeleteFolder("Extensions/GooglePlayCommon/Social/Facebook");
					FileStaticAPI.DeleteFile("Extensions/AndroidNative/xExample/Scripts/Social/FacebookAndroidUseExample.cs");

				}

			}
			GUI.enabled = true;




		}
	}
	

	private void PlayServiceSettings() {
		EditorGUILayout.HelpBox("(Optional) PlayService Parameters", MessageType.None);
		AndroidNativeSettings.Instance.ShowPSSettings = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowPSSettings, "PlayService Settings");
		if(AndroidNativeSettings.Instance.ShowPSSettings) {


			EditorGUILayout.LabelField("API:");
			EditorGUI.indentLevel++;

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(GamesApiLabel);
			settings.EnableGamesAPI	 	= EditorGUILayout.Toggle(settings.EnableGamesAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(AppSateApiLabel);
			settings.EnableAppStateAPI	 	= EditorGUILayout.Toggle(settings.EnableAppStateAPI);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;


			EditorGUILayout.LabelField("Auto Image Loading:");

			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Profile Icons");
			settings.LoadProfileIcons	 	= EditorGUILayout.Toggle(settings.LoadProfileIcons);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Profile Hi-res Images");
			settings.LoadProfileImages	 	= EditorGUILayout.Toggle(settings.LoadProfileImages);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Event Icons");
			settings.LoadEventsIcons	 	= EditorGUILayout.Toggle(settings.LoadEventsIcons);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest Icons");
			settings.LoadQuestsIcons	 	= EditorGUILayout.Toggle(settings.LoadQuestsIcons);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest Banners");
			settings.LoadQuestsImages	 	= EditorGUILayout.Toggle(settings.LoadQuestsImages);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
		}

	}

	private void BillingSettings() {
		EditorGUILayout.HelpBox("(Optional) In-app Billing Parameters", MessageType.None);
		AndroidNativeSettings.Instance.ShowStoreKitParams = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowStoreKitParams, "Billing Settings");
		if(AndroidNativeSettings.Instance.ShowStoreKitParams) {

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Base64KeyLabel);
			settings.base64EncodedPublicKey	 	= EditorGUILayout.TextField(settings.base64EncodedPublicKey);
			EditorGUILayout.EndHorizontal();


			if(settings.InAppProducts.Count == 0) {
				EditorGUILayout.HelpBox("No products added", MessageType.Warning);
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
		}
	}



	
	
	private void AboutGUI() {

		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		
		SelectableLabelField(SdkVersion, AndroidNativeSettings.VERSION_NUMBER);
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
		EditorUtility.SetDirty(AndroidNativeSettings.Instance);
		#endif
	}
	
	
}
