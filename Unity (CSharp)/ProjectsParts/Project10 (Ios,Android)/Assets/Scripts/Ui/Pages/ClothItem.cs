using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.View {
	public class ClothItem : RoundItem {
		protected override void ShowData() {
			base.ShowData();

			int level = _product.GetLevel();

			if (level == 2) {
				boughtPanel.SetActive(true);
				pricePanel.SetActive(false);
				boungText.color = textBoung;
				boungText.text = LanguageManager.GetTranslate("shop_Wearing"); // "Надето";
				fillRound.GetComponent<Image>().color = roundBoung;
			} else if (level == 1) {
				boughtPanel.SetActive(true);
				pricePanel.SetActive(false);
				boungText.color = textByed;
				boungText.text = LanguageManager.GetTranslate("shop_Bought"); //"Куплено";
				fillRound.GetComponent<Image>().color = roundByed;
			} else if (level == 0) {
				boughtPanel.SetActive(false);
				pricePanel.SetActive(true);
				//pricePanel.GetComponent<Image>().color = roundActive;
				coinsText.text = _product.GetPrice().coins.ToString();

			}

		}
	}
}