using Game.Scripts.Providers.Shop.Products;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop
{
	public abstract class ProductButtonUi : MonoBehaviour
	{
		[HideInInspector] public UnityAction<IProduct> OnBuyTouchEvent;

		[SerializeField] protected Button _buyButton;
		[SerializeField] protected TMP_Text _titleLabel;
		[SerializeField] protected TMP_Text _priceLabel;

		protected IProduct _product;

		public abstract string Type { get; }

		public virtual void SetProduct(IProduct product)
		{
			_product = product;
			FillUi();
		}
		protected virtual void FillUi() { }

		protected void SetPrice(float priceValue)
		{
			if (_priceLabel != null)
				_priceLabel.text = string.Format("${0:f2}", priceValue);
		}

		public void BuyButtonTouch() => OnBuyTouchEvent?.Invoke(_product);
	}
}
