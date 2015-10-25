using UnityEngine;
using System.Collections;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSNativeAppEvents : EventDispatcher {

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE

	[DllImport ("__Internal")]
	private static extern void _ISNsubscribe();
	#endif

	public const string APPLICATION_DID_ENTER_BACKGROUND 		= "applicationDidEnterBackground";
	public const string APPLICATION_DID_BECOME_ACTIVE 			= "applicationDidBecomeActive";
	public const string APPLICATION_DID_RECEIVE_MEMORY_WARNING 	= "applicationDidReceiveMemoryWarning";
	public const string APPLICATION_WILL_RESIGN_ACTIVE 	        = "applicationWillResignActive";
	public const string APPLICATION_WILL_TERMINATE 	            = "applicationWillTerminate";



	public static IOSNativeAppEvents _instance = null;


	public static IOSNativeAppEvents instance {
		get {
			if(_instance == null) {
				GameObject go =  new GameObject("IOSNativeAppEvents");
				DontDestroyOnLoad(go);
				_instance =  go.AddComponent<IOSNativeAppEvents>();
			}

			return _instance;
		}
	}

	
	void Awake() {
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_ISNsubscribe();
		#endif
	}


	private void applicationDidEnterBackground() {
		dispatch(APPLICATION_DID_ENTER_BACKGROUND);
	}
	
	private void applicationDidBecomeActive() {
		dispatch(APPLICATION_DID_BECOME_ACTIVE);
	}
	
	private void applicationDidReceiveMemoryWarning() {
		dispatch(APPLICATION_DID_RECEIVE_MEMORY_WARNING);
	}
	
	
	private void applicationWillResignActive() {
		dispatch (APPLICATION_WILL_RESIGN_ACTIVE);
	}
	
	
	private void applicationWillTerminate() {
		dispatch (APPLICATION_WILL_TERMINATE);
	}


}

