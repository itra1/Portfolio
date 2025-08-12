using Shop.Products;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.View {

	public class RoundItem : ExEvent.EventBehaviour {
		
		protected Product _product;

		public Product Product {
			get { return _product; }
		}
		public Text coinsText;
		public RectTransform coinsTextParent;
		public Text boungText;
		
		public Image icon;
		
		public GameObject fillRound;
		public GameObject roundSprite;
		public GameObject pricePanel;
		public GameObject boughtPanel;
		
		public Color roundActive;
		public Color roundByed;
		public Color roundBoung;
		public Color textByed;
		public Color textBoung;

		private bool isSelected = false;

		public System.Action<Product> OnClick;

		[ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.CoinsChange))]
		public void CoinsUpdate(ExEvent.RunEvents.CoinsChange eventData) {
			SetSelect(_product);
		}

		public void SetProduct(Product product) {
			this._product = product;
			ShowData();
		}

		protected virtual void OnEnable() {
			boughtPanel.SetActive(false);
			pricePanel.SetActive(false);
		}

		public virtual void SetSelect(Product product) {
			isSelected = _product.id == product.id;
			roundSprite.SetActive(isSelected);
		}

		protected virtual void ShowData() {

			if (icon != null) {
				icon.sprite = _product.sprite;
				
				if (_product.sprite.rect.width > _product.sprite.rect.height) {
					icon.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 120 / icon.sprite.rect.width * icon.sprite.rect.height);
				} else {
					icon.GetComponent<RectTransform>().sizeDelta = new Vector2(120 / icon.sprite.rect.height * icon.sprite.rect.width, 120);
				}
			}


		}

		public void Click() {

			if (OnClick != null) OnClick(_product);

		}

	}

}