////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class IOSInAppPurchaseManager : EventDispatcher {


	public const string APPLE_VERIFICATION_SERVER   = "https://buy.itunes.apple.com/verifyReceipt";
	public const string SANDBOX_VERIFICATION_SERVER = "https://sandbox.itunes.apple.com/verifyReceipt";



	public const string PRODUCT_BOUGHT 		= "product_bought";
	public const string TRANSACTION_FAILED 	= "transaction_failed";
	public const string RESTORE_TRANSACTION_FAILED 	= "restore_transaction_failed";
	public const string RESTORE_TRANSACTION_COMPLETE 	= "restore_transaction_complete";
	

	public const string VERIFICATION_RESPONSE 	= "verification_response";
	public const string STORE_KIT_INITIALIZED	= "store_kit_initialized";
	public const string STORE_KIT_INITI_FAILED	= "store_kit_init_failed";
	
	
	private bool _IsStoreLoaded = false;
	private bool _IsWaitingLoadResult = false;
	private bool _IsInAppPurchasesEnabled = true;
	private static int _nextId = 1;
	
	private List<string> _productsIds =  new List<string>();
	private List<ProductTemplate> _products    =  new List<ProductTemplate>();
	private Dictionary<int, IOSStoreProductView> _productsView =  new Dictionary<int, IOSStoreProductView>(); 
	
	
	private static IOSInAppPurchaseManager _instance;
	private static string lastPurchasedProdcut;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static IOSInAppPurchaseManager instance {
		get {
			if(_instance ==  null) {
				GameObject go =  new GameObject("IOSInAppPurchaseManager");
				DontDestroyOnLoad(go);
				_instance =  go.AddComponent<IOSInAppPurchaseManager>();
			}
			
			return _instance;
		}
	}



	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void loadStore() {


		if(_IsWaitingLoadResult || _IsStoreLoaded) {
			return;
		}

		_IsWaitingLoadResult = true;
		
		foreach(string pid in IOSNativeSettings.Instance.InAppProducts) {
			addProductId(pid);
		}
		
		string ids = "";
		int len = _productsIds.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				ids += ",";
			}
			
			ids += _productsIds[i];
		}
		
		IOSNativeMarketBridge.loadStore(ids);

		
	}


	public void buyProduct(string productId) {
		if(!_IsStoreLoaded) {

			Debug.LogWarning("buyProduct shouldn't be called before store kit initialized"); 
			SendTransactionFailEvent(productId, "Store kit not yet initialized");

			return;
		} else {
			if(!_IsInAppPurchasesEnabled) {
				SendTransactionFailEvent(productId, "In App Purchases Disabled");
			}
		}

		IOSNativeMarketBridge.buyProduct(productId);
	}
	
	public void addProductId(string productId) {
		if(_productsIds.Contains(productId)) {
			return;
		}

		_productsIds.Add(productId);
	}

	public ProductTemplate GetProductById(string id) {
		foreach(ProductTemplate tpl in products) {
			if(tpl.id.Equals(id)) {
				return tpl;
			}
		}

		return null;
	}
	
	public void restorePurchases() {

		if(!_IsStoreLoaded || !_IsInAppPurchasesEnabled) {
			dispatch(RESTORE_TRANSACTION_FAILED);
			return;
		}

		IOSNativeMarketBridge.restorePurchases();
	}

	public void verifyLastPurchase(string url) {
		IOSNativeMarketBridge.verifyLastPurchase (url);
	}

	public void RegisterProductView(IOSStoreProductView view) {
		view.SetId(nextId);
		_productsView.Add(view.id, view);
	}
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public List<ProductTemplate> products {
		get {
			return _products;
		}
	}

	public bool IsStoreLoaded {
		get {
			return _IsStoreLoaded;
		}
	}

	public bool IsInAppPurchasesEnabled {
		get {
			return _IsInAppPurchasesEnabled;
		}
	}

	public bool IsWaitingLoadResult {
		get {
			return _IsWaitingLoadResult;
		}
	}

	public static int nextId {
		get {
			_nextId++;
			return _nextId;
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void onStoreKitStart(string data) {
		int satus = System.Convert.ToInt32(data);
		if(satus == 1) {
			_IsInAppPurchasesEnabled = true;
		} else {
			_IsInAppPurchasesEnabled = false;
		}
	}

	private void OnStoreKitInitFailed(string data) {


		string[] storeData;
		storeData = data.Split("|" [0]);
		ISN_Error e =  new ISN_Error();
		e.code = System.Convert.ToInt32(storeData[0]);
		e.description = storeData[1];


		_IsStoreLoaded = false;
		_IsWaitingLoadResult = false;
		dispatch(STORE_KIT_INITI_FAILED, e);

	}
	
	public void onStoreDataReceived(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no products avaiable: " + _products.Count.ToString());
			dispatch (STORE_KIT_INITIALIZED);
			return;
		}


		string[] storeData;
		storeData = data.Split("|" [0]);
		
		for(int i = 0; i < storeData.Length; i+=7) {
			ProductTemplate tpl =  new ProductTemplate();
			tpl.id 				= storeData[i];
			tpl.title 			= storeData[i + 1];
			tpl.description 	= storeData[i + 2];
			tpl.localizedPrice 	= storeData[i + 3];
			tpl.price 			= storeData[i + 4];
			tpl.currencyCode 	= storeData[i + 5];
			tpl.currencySymbol 	= storeData[i + 6];
			_products.Add(tpl);
		}
		
		Debug.Log("InAppPurchaseManager, tottal products loaded: " + _products.Count.ToString());
		_IsStoreLoaded = true;
		_IsWaitingLoadResult = false;
		dispatch (STORE_KIT_INITIALIZED);
	}
	
	public void onProductBought(string array) {

		string[] data;
		data = array.Split("|" [0]);
		IOSStoreKitResponse response = new IOSStoreKitResponse ();
		response.productIdentifier = data [0];
		response.receipt = data [1];

		lastPurchasedProdcut = response.productIdentifier;
		dispatch(PRODUCT_BOUGHT, response);
	}
	
	public void onTransactionFailed(string array) {

		string[] data;
		data = array.Split("|" [0]);

		SendTransactionFailEvent(data [0], data [1]);
	}
	
	
	public void onVerificationResult(string array) {

		string[] data;
		data = array.Split("|" [0]);

		IOSStoreKitVerificationResponse response = new IOSStoreKitVerificationResponse ();
		response.status = System.Convert.ToInt32(data[0]);
		response.originalJSON = data [1];
		response.receipt = data [2];
		response.productIdentifier = lastPurchasedProdcut;

		dispatch (VERIFICATION_RESPONSE, response);

	}

	public void onRestoreTransactionFailed(string array) {
		dispatch(RESTORE_TRANSACTION_FAILED);
	}

	public void onRestoreTransactionComplete(string array) {
		dispatch(RESTORE_TRANSACTION_COMPLETE);
	}



	private void OnProductViewLoaded(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoaded();
		}
	}

	private void OnProductViewLoadedFailed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnContentLoadFailed();
		}
	}

	private void OnProductViewDismissed(string viewId) {
		int id = System.Convert.ToInt32(viewId);
		if(_productsView.ContainsKey(id)) {
			_productsView[id].OnProductViewDismissed();
		}
	}

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void SendTransactionFailEvent(string productIdentifier, string error) {
		IOSStoreKitResponse response = new IOSStoreKitResponse ();
		response.productIdentifier = productIdentifier;
		response.error =  error;
		
		
		dispatch(TRANSACTION_FAILED, response);
	}
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
