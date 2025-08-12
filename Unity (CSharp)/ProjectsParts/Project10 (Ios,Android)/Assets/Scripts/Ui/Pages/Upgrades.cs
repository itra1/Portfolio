using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shop.View.Pages {
	public class Upgrades : Page {

		public ShopItemCotnroller prefab;
		public Shop.Products.ProductType productType;

		private List<ShopItemCotnroller> productList = new List<ShopItemCotnroller>();

		private void Start() {
			SpawnElements();
		}

		private void SpawnElements() {

			if (productList.Count > 0) {
				productList.ForEach(x=>Destroy(x.gameObject));
				productList.Clear();
			}

			prefab.gameObject.SetActive(false);
			List<Products.Product> sourceProduct = ShopManager.Instance.GetProductList(productType);

			float positionY = -127;

			sourceProduct.ForEach(prod => {

				GameObject inst = Instantiate(prefab.gameObject);
				inst.SetActive(true);
				inst.transform.SetParent(parentItems);
				inst.transform.localScale = Vector3.one;
				RectTransform rectInst = inst.GetComponent<RectTransform>();
				rectInst.anchoredPosition = new Vector2(0, positionY);
				rectInst.sizeDelta = prefab.GetComponent<RectTransform>().sizeDelta;
				positionY -= 180;

				ShopItemCotnroller instController = inst.GetComponent<ShopItemCotnroller>();
				productList.Add(instController);
				instController.SetProduct(prod);
				parentItems.sizeDelta = new Vector2(parentItems.sizeDelta.x, -positionY+10);
			});

		}

	}
}