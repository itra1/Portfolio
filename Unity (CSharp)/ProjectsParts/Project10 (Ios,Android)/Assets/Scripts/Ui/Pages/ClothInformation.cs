using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.View.Pages {

	public class ClothInformation : MonoBehaviour {

		public Text titleText;
		public Text descriptionText;
		public Image icone;

		private Shop.Products.Product _product;

		public void SetProduct(Shop.Products.Product product) {

			this._product = product;
			
			titleText.text = LanguageManager.GetTranslate(product.displayTitle);
			descriptionText.text = LanguageManager.GetTranslate(product.displayDescription);

			icone.sprite = product.sprite;
			icone.GetComponent<AspectRatioFitter>().aspectRatio = product.sprite.rect.width / product.sprite.rect.height;

		}

	}
}