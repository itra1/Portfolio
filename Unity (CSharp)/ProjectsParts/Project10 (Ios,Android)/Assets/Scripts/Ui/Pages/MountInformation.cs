using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.View.Pages {
	public class MountInformation : MonoBehaviour {

		public Text titleText;
		public Text descriptionText;

		public void SetProduct(Shop.Products.Product product) {
      
			titleText.text = LanguageManager.GetTranslate(product.displayTitle);
			descriptionText.text = LanguageManager.GetTranslate(product.displayDescription);
			
		}
	}
}