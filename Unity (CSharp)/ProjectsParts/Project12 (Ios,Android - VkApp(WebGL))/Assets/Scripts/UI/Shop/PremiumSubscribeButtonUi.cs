using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Products;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Shop
{
	public class PremiumSubscribeButtonUi : ProductButtonUi
	{
		[SerializeField] private TMP_Text _songCountLabel;
		[SerializeField] private TMP_Text _oldPriceLabel;

		public override string Type => ProductType.PremiumSubscribe;

		protected override void FillUi()
		{
			var currentProduct = (PremiumSubscribeProduct) _product;
			_titleLabel.text = currentProduct.Prooperty.Title;
			SetPrice(currentProduct.Prooperty.Price.Value);
			SetSongCount(currentProduct.Prooperty.SongCount);
			SetOldPrice(currentProduct.Prooperty.Price.OldValue);
		}

		private void SetSongCount(int count) =>
			_songCountLabel.text = $"{count} <size=65%>songs/monts";

		private void SetOldPrice(float priceValue) =>
				_oldPriceLabel.text = string.Format("<s>${0:f2}</s>", priceValue);
	}
}
