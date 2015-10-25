
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GoogleMobileAdSettings))]
public class GoogleMobileAdSettingsEditor : Editor {


	
	
	GUIContent IOS_UnitAdId  	 = new GUIContent("Banners Ad Unit Id [?]:",  "IOS Banners Ad Unit Id ");
	GUIContent IOS_InterstAdId   = new GUIContent("Interstitials Ad Unit Id [?]:", "IOS Interstitials Ad Unit Id");

	GUIContent Android_UnitAdId  	 = new GUIContent("Banners Ad Unit Id [?]:",  "Android Banners Ad Unit Id ");
	GUIContent Android_InterstAdId   = new GUIContent("Interstitials Ad Unit Id [?]:", "Android Interstitials Ad Unit Id");

	GUIContent WP8_UnitAdId  	 = new GUIContent("Banners Ad Unit Id [?]:",  "WP8 Banners Ad Unit Id ");
	GUIContent WP8_InterstAdId   = new GUIContent("Interstitials Ad Unit Id [?]:", "WP8 Interstitials Ad Unit Id");



	
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");


	private GoogleMobileAdSettings settings;

	private bool IsIOSSettinsOpened = true;
	private bool IsAndroidSettinsOpened = true;
	private bool IsWP8SettinsOpened = true;



	private const string version_info_file = "Plugins/StansAssets/Versions/GMA_VersionInfo.txt"; 


	public override void OnInspectorGUI() {
		settings = target as GoogleMobileAdSettings;

		GUI.changed = false;



		GeneralOptions();
		EditorGUILayout.Space();
		MainSettings();
		EditorGUILayout.Space();
		AboutGUI();
	

		if(GUI.changed) {
			DirtyEditor();
		}
	}



	public static bool IsInstalled {
		get {
			if(FileStaticAPI.IsFileExists(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "androidnative.jar") && FileStaticAPI.IsFileExists(PluginsInstalationUtil.IOS_DESTANATION_PATH + "GoogleMobileAdBanner.h")) {
				return true;
			} else {
				return false;
			}
		}
	}
	
	public static bool IsUpToDate {
		get {
			if(GoogleMobileAdSettings.VERSION_NUMBER.Equals(DataVersion)) {
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
		FileStaticAPI.Write(version_info_file, GoogleMobileAdSettings.VERSION_NUMBER);

	
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
				PluginsInstalationUtil.IOS_InstallPlugin();
				UpdateVersionInfo();
			}
			GUI.color = c;
			EditorGUILayout.EndHorizontal();
		}
		
		if(IsInstalled) {
			if(!IsUpToDate) {
				EditorGUILayout.HelpBox("Update Required \nResources version: " + DataVersion + " Plugin version: " + GoogleMobileAdSettings.VERSION_NUMBER, MessageType.Warning);
				
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				Color c = GUI.color;
				GUI.color = Color.cyan;
				if(GUILayout.Button("Update to " + GoogleMobileAdSettings.VERSION_NUMBER,  GUILayout.Width(250))) {
					PluginsInstalationUtil.Android_UpdatePlugin();
					PluginsInstalationUtil.IOS_UpdatePlugin();
					UpdateVersionInfo();
				}
				
				GUI.color = c;
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				
			} else {
				EditorGUILayout.HelpBox("Google Mobile Ad Plugin v" + GoogleMobileAdSettings.VERSION_NUMBER + " is installed", MessageType.Info);
				
			}
		}
		
		
		EditorGUILayout.Space();
		
	}


	public void MainSettings() {

		EditorGUILayout.HelpBox("Google Mobile Ad Plugin", MessageType.None);
		EditorGUILayout.Space();

		IsIOSSettinsOpened = EditorGUILayout.Foldout(IsIOSSettinsOpened, "IOS");
		if(IsIOSSettinsOpened) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(IOS_UnitAdId);
			settings.IOS_BannersUnitId	 	= EditorGUILayout.TextField(settings.IOS_BannersUnitId);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(IOS_InterstAdId);
			settings.IOS_InterstisialsUnitId	 	= EditorGUILayout.TextField(settings.IOS_InterstisialsUnitId);
			EditorGUILayout.EndHorizontal();
		}


		IsAndroidSettinsOpened = EditorGUILayout.Foldout(IsAndroidSettinsOpened, "Android");
		if(IsAndroidSettinsOpened) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Android_UnitAdId);
			settings.Android_BannersUnitId	 	= EditorGUILayout.TextField(settings.Android_BannersUnitId);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Android_InterstAdId);
			settings.Android_InterstisialsUnitId	 	= EditorGUILayout.TextField(settings.Android_InterstisialsUnitId);
			EditorGUILayout.EndHorizontal();
		}



		IsWP8SettinsOpened = EditorGUILayout.Foldout(IsWP8SettinsOpened, "WP8");
		if(IsWP8SettinsOpened) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(WP8_UnitAdId);
			settings.WP8_BannersUnitId	 	= EditorGUILayout.TextField(settings.WP8_BannersUnitId);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(WP8_InterstAdId);
			settings.WP8_InterstisialsUnitId	 	= EditorGUILayout.TextField(settings.WP8_InterstisialsUnitId);
			EditorGUILayout.EndHorizontal();
		}
	}



	private void AboutGUI() {

		EditorGUILayout.HelpBox("Version Info", MessageType.None);
		EditorGUILayout.Space();
		
		SelectableLabelField(SdkVersion, GoogleMobileAdSettings.VERSION_NUMBER);
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
		EditorUtility.SetDirty(GoogleMobileAdSettings.Instance);
		#endif
	}
	
	
}
