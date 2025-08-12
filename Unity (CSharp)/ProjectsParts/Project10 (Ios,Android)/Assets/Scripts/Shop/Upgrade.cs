using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shop.Products {

	public class Upgrade : Product {
		
		public Price[] priceLevel;

		public override void Bye(Action<Product, bool> callback) {
			if (!IsBye() && !ByePossible()) {
				if (callback != null) callback(this, false);
				return;
			}

			var price = GetPrice();

			UserManager.coins -= price.coins;

			PlayerPrefs.SetInt(id, (PlayerPrefs.GetInt(id, 0)+1));

			if (callback != null) callback(this, true);
		}

		public override bool ByePossible() {
			int level = PlayerPrefs.GetInt(id, 0);
			if (level == priceLevel.Length)
				return false;
			return UserManager.coins >= priceLevel[level].coins;
		}

		public override Price GetPrice() {
			int level = PlayerPrefs.GetInt(id, 0);
			return priceLevel[level];
		}

		public override bool IsBye() {
			int level = PlayerPrefs.GetInt(id, 0);
			return level >= priceLevel.Length;
		}
	}

}