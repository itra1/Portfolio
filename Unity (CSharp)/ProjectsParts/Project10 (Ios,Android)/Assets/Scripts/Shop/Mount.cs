
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shop.Products {

	public sealed class Mount : Product {
		
		public Price[] priceLevel;

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
			int level = PlayerPrefs.GetInt(id, -1);
			return UserManager.coins >= priceLevel[level + 1].coins;
		}

		public override Price GetPrice() {
			int level = PlayerPrefs.GetInt(id, -1);
			return priceLevel[level + 1];
		}

		public override bool IsBye() {
			int level = PlayerPrefs.GetInt(id, -1);
			return level >= priceLevel.Length;
		}
	}

}