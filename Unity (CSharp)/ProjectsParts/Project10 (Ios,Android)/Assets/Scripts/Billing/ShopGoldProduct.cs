using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Billing {

	public class ShopGoldProduct : MonoBehaviour {

		public GoldBillingProduct product;
		public Text priceText;
		public Text localText;
		public RectTransform priceRect;

		[SerializeField]
		private GameObject loadBlack;


		private void OnEnable() {

			//try {
			//	regex = new Regex(@"[0-9]+.*[0-9]+");
			//	Match match = regex.Match(product.product.metadata.localizedPriceString);

			//	if (match.Success) {
			//		goldProduct[i].priceText.text = match.Groups[0].Value.Trim();
			//	}
			//} catch {
			//}



			//try {
			//	regex = new Regex(@"(?<=[0-9]+.*[0-9]+)\D*$");
			//	Match match = regex.Match(localprice);

			//	if (match.Success) {
			//		goldProduct[i].localText.text = match.Groups[0].Value.Trim();
			//	}
			//} catch {
			//}
			priceText.text = product.product.metadata.localizedPriceString;
			//localText.text = product.product.metadata.isoCurrencyCode;

			priceRect.sizeDelta = new Vector2(priceText.preferredWidth + localText.preferredWidth + 10, priceRect.sizeDelta.y);

			//summLenght = (goldProduct[i].priceText.preferredWidth + goldProduct[i].localText.preferredWidth + 10) / 2;
			//priceText.rectTransform.localPosition = new Vector3(-summLenght + (goldProduct[i].priceText.preferredWidth / 2), goldProduct[i].priceText.rectTransform.localPosition.y, goldProduct[i].priceText.rectTransform.localPosition.z);
			//localText.rectTransform.localPosition = new Vector3(summLenght - (goldProduct[i].localText.preferredWidth / 2), goldProduct[i].localText.rectTransform.localPosition.y, goldProduct[i].localText.rectTransform.localPosition.z);


		}


		public static bool waitAnswer;

		public void Bye() {

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

	}

}
