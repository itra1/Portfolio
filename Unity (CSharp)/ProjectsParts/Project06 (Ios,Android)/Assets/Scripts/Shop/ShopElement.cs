using System;
using UnityEngine.UI;

	public class ShopElement : ExEvent.EventBehaviour {

		public Action<ShopProductBehaviour> OnClick;
		public Action<ShopProductBehaviour> OnBye;

		private ShopProductBehaviour _product;

		public Image icon;
		public Text title;
		public Text count;
		public Text price;
		public Text description;

		public void SetProduct(ShopProductBehaviour product) {
			this._product = product;

			if (title != null) title.text = product.name;
			if (price != null) price.text = product.price.ToString();
			if (description != null) description.text = product.description;
			UpdateParams();

			if (icon != null) {

				if (product.icon != null) {
					icon.sprite = product.icon;
				}else if (product is WeaponShopProduct) {
					icon.sprite = Game.User.UserWeapon.Instance.weaponsManagers.Find(x => x.weaponType == (product as WeaponShopProduct).weaponType).IconActive;
				}
			}
		}

		[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnBye))]
		private void ProductBye(ExEvent.GameEvents.OnBye prod) {
			if (prod.product.title == this._product.title)
				UpdateParams();
		}

		private void UpdateParams() {
			if (count != null) count.text = this._product.count.ToString();
		}

		public void ByeButton() {
			UIController.ClickPlay();
			if (OnBye != null) OnBye(_product);
		}

		public void ClickElement() {
			UIController.ClickPlay();
			if (OnClick != null) OnClick(_product);
		}


	}