using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shop.Products {
	public class Gadget : Product {
		
		public Price price;
		
		public override void Bye(Action<Product, bool> callback) {
			if (!IsBye() && !ByePossible()) {
				if (callback != null) callback(this, false);
				return;
			}

			var price = GetPrice();

			UserManager.coins -= price.coins;

			PlayerPrefs.SetInt(id, (PlayerPrefs.GetInt(id, 0) + 1));

			if (callback != null) callback(this, true);
		}

		public override bool ByePossible() {
			return UserManager.coins >= price.coins;
		}

		public override Price GetPrice() {
			return price;
		}

		public override bool IsBye() {
			return false;
		}



	}
}