using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Billing;
using UnityEngine.UI;


/// <summary>
/// Магазин реального бабла
/// </summary>
public class GoldShop : MonoBehaviour {
	
	[SerializeField]
	private ShopController shop;

	[SerializeField]
	private GameObject loadBlack;

	void OnEnable() {
//		loadBlack.SetActive(false);
		
//		string localprice = "";
//		Regex regex;
//		float summLenght = 0;

//		//UiController.AddLog("Инифиализация GoldShop");
//		//UiController.AddLog("Флаг загрузки " + BillingController.productDownload);
//		//UiController.AddLog("Количество продуктов " + goldProduct.Count);

//		//if (BillingController.productDownload) {
//		if (true) {
//			for (int i = 0; i < goldProduct.Count; i++) {
//#if PLUGIN_SDKBOX
//        if(BillingController.instance.sdkBox) {
//          //for(int j = 0; j < BillingController.productSDK.Length; j++) {
//          //  if(BillingController.productSDK[j].id == goldProduct[i].productType.ToString())
//          //    localprice = BillingController.productSDK[j].price;
//          //}
//          UiController.AddLog("Прайс по SDKBOS" + localprice);
//        }
//#endif
//#if PLUGIN_VOXELBUSTERS
//				//if(!BillingController.Instance.sdkBox) {
//				//  for(int j = 0; j < BillingController.product.Length; j++) {
//				//    if(BillingController.product[j].ProductIdentifier == goldProduct[i].productType.ToString())
//				//      localprice = BillingController.product[j].LocalizedPrice;
//				//  }
//				//  UiController.AddLog("Прайс по VoxelBusters" + localprice);
//				//}
//#endif

//				try {
//					regex = new Regex(@"[0-9]+.*[0-9]+");
//					Match match = regex.Match(localprice);

//					if (match.Success) {
//						goldProduct[i].priceText.text = match.Groups[0].Value.Trim();
//					}
//				} catch {
//				}
//				try {
//					regex = new Regex(@"(?<=[0-9]+.*[0-9]+)\D*$");
//					Match match = regex.Match(localprice);

//					if (match.Success) {
//						goldProduct[i].localText.text = match.Groups[0].Value.Trim();
//					}
//				} catch {
//				}

//				summLenght = (goldProduct[i].priceText.preferredWidth + goldProduct[i].localText.preferredWidth + 10) / 2;
//				goldProduct[i].priceText.rectTransform.localPosition = new Vector3(-summLenght + (goldProduct[i].priceText.preferredWidth / 2), goldProduct[i].priceText.rectTransform.localPosition.y, goldProduct[i].priceText.rectTransform.localPosition.z);
//				goldProduct[i].localText.rectTransform.localPosition = new Vector3(summLenght - (goldProduct[i].localText.preferredWidth / 2), goldProduct[i].localText.rectTransform.localPosition.y, goldProduct[i].localText.rectTransform.localPosition.z);
//				//priceText[i].rectTransform.localPosition.
//			}
//		}
	}

	bool waitAnswer;

	public void ByeElement(GoldBillingProduct product) {

		if (waitAnswer) return;
		waitAnswer = true;
		UiController.ClickButtonAudio();
		loadBlack.SetActive(true);

		BillingController.Instance.ByeProduct(product, () => {

			loadBlack.SetActive(false);
			
			YAppMetrica.Instance.ReportEvent("Инап: покупка " + product.productId);
			GAnalytics.Instance.LogEvent("Инап", "Покупка", product.productId, 1);
			
			//BillingController.Instance.ByeProduct(product, null);

			//foreach (GoldProduct one in goldProduct)
			//	if (one.productType.ToString() == numBilling.ToString()) shop.ByeItem(one.productType);

			waitAnswer = false;

		});

	}

	void ConfirmBye(string productName, bool flag) {

	}
}
