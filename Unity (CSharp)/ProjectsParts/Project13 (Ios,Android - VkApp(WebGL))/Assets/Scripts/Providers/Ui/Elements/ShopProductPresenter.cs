using Game.Providers.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Providers.Ui.Elements
{
	public class ShopProductPresenter : MonoBehaviour
	{
		[SerializeField] private Image _iconeImage;
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private TMP_Text _priceLabel;
		[SerializeField] private Button _selfButton;

		private IShopProduct _product;

		public void SetData(IShopProduct product)
		{
			_product = product;

			_iconeImage.sprite = _product.Icone;
			_titleLabel.text = _product.Title;
			SetPrice(_product.Price);

			_selfButton.onClick.RemoveAllListeners();
			_selfButton.onClick.AddListener(SelfButtonTouch);
		}

		private void SelfButtonTouch()
		{
			_ = _product.Buy();
		}

		private void SetPrice(ulong value)
		{
			_priceLabel.text = $"<sprite=0>{value}";
		}
	}
}
