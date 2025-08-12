using System.Collections;
using System.Collections.Generic;
using Shop.Products;
using UnityEngine;

namespace Shop {

	public class ShopManager : Singleton<ShopManager> {

		private void Start() {}
		
		public List<Product> productList;

		public Product GetProduct(string productId) {
			return productList.Find(x => x.id == productId);
		}

		public List<Product> GetProductList(ProductType type) {
			return productList.FindAll(x => (x.type & type) != 0);
		}

	}

}