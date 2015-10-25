﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WP8InAppPurchasesManager : EventDispatcherBase {
	
	private static WP8InAppPurchasesManager _instance = null;
	
	private List<WP8ProductTemplate> _products =  new List<WP8ProductTemplate>();
		
	public const string  INITIALIZED = "PRODUCTS_DETAILS_LOADED";
	public const string  PRODUCT_PURCHASE_FINISHED = "PRODUCT_PURCHASE_FINISHED";

	public static WP8InAppPurchasesManager instance {
		get {
			if(_instance == null) {
				_instance =  new WP8InAppPurchasesManager();
			}
			
			return _instance;
		}
	}
	
	public void init() {
		#if UNITY_WP8 || UNITY_METRO
		WP8Native.InAppPurchases.productsInit(ProductsDetailsDelegate);
		#endif
	}
	
	public void purchase(string productId) {
		#if UNITY_WP8 || UNITY_METRO
		WP8Native.InAppPurchases.BuyItem(productId, ProductPurchseDelegate);
		#endif
	}
		
	public List<WP8ProductTemplate> products  {
		get {
			return _products;
		}
	}

	public WP8ProductTemplate GetProductById(string id) {
		foreach(WP8ProductTemplate p in _products) {
			if(p.ProductId.Equals(id)) {
				return p;
			}
		}

		return null;
	} 
		
	private void ProductsDetailsDelegate(string data) {
				
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, you have no avaiable products");
			dispatch(INITIALIZED, _products);
			return;
		}
		
		string[] storeData;
		storeData = data.Split("|" [0]);
		
		for ( int i = 0; i < storeData.Length; i += 7 ) {
			WP8ProductTemplate tpl =  new WP8ProductTemplate();
			tpl.ImgURL = storeData[i];
			tpl.Name = storeData[i + 1];
			tpl.ProductId = storeData[i + 2];
			tpl.Price = storeData[i + 3];
			tpl.Type = (WP8PurchaseProductType)Enum.Parse(typeof(WP8PurchaseProductType), storeData[i + 4]);
			tpl.Description = storeData[i + 5];
			tpl.isPurchased = (Boolean)Boolean.Parse(storeData[i + 6]);
			
			_products.Add(tpl);
			
		}
		dispatch(INITIALIZED, _products);
	}
	
	private void ProductPurchseDelegate(string data) {
		
		string[] storeData;
		storeData = data.Split("|" [0]);
		
		WP8PurchaseCodes code = (WP8PurchaseCodes)Enum.Parse(typeof(WP8PurchaseCodes), storeData[0]);
		string info_str = storeData[1];
		string productID = storeData[2];
		
		if ( code == WP8PurchaseCodes.SUCCSES ) {
			foreach ( WP8ProductTemplate product in _products) {
				if ( product.ProductId == productID && product.Type == WP8PurchaseProductType.Durable ) {
					product.isPurchased = true;
				}
			}
		}
		
		WP8PurchseResponce recponce =  new WP8PurchseResponce(code, info_str);
		recponce.productId = productID;
		dispatch(PRODUCT_PURCHASE_FINISHED, recponce);
	}
}
