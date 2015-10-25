using UnityEngine;
using System.Collections;

public class AndroidNotificationManager  {
	public const int LENGTH_SHORT = 0; // 2 seconds 
	public const int LENGTH_LONG  = 1; // 3.5 seconds



	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public static void ShowToastNotification(string text) {
		ShowToastNotification (text, LENGTH_SHORT);
	}
	
	public static void ShowToastNotification(string text, int duration) {
		AndroidNative.ShowToastNotification (text, duration);
	}

	public static void ScheduleLocalNotification(string title, string message, int seconds) {
		AndroidNative.ScheduleLocalNotification(title, message, seconds);
	}


	public static void CanselLocalNotification(string title, string message) {
		AndroidNative.CanselLocalNotification(title, message);
	}

}

