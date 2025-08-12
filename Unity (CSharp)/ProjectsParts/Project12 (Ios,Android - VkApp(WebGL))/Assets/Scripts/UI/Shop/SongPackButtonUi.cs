using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Products;
using Game.Scripts.UI.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop
{
	public class SongPackButtonUi : ProductButtonUi
	{
		[SerializeField] private TMP_Text _countLabel;
		[SerializeField] private Image _backgroundImage;

		private SongsPackProduct _currentProduct;

		public override string Type => ProductType.SongsPack;

		private void SetColor(ProductButtonColorsUiItem colorSetting)
		{
			if (colorSetting == null)
				return;

			_backgroundImage.sprite = colorSetting.IconeBack;
			_priceLabel.color = colorSetting.Color;
		}

		protected override void FillUi()
		{
			var currentProduct = (SongsPackProduct) _product;
			_titleLabel.text = currentProduct.Prooperty.Title;
			SetPrice(currentProduct.Prooperty.Price.Value);
			SetCountLabel(currentProduct.Prooperty.SongCount);
			SetColor(currentProduct.ColorData);
		}

		private void SetCountLabel(int count)
		{
			_countLabel.text = $"{count}\r\n<font=Rubik-Regular SDF><size=70%><color=#ffffff66>songs";
		}
	}
}
