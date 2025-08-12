using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Products;
using itra.Attributes;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Shop
{
	[PrefabName(ProductType.WelcomeBundle)]
	public class WelcomeBundleButtonUi : ProductButtonUi
	{
		[SerializeField] private TMP_Text _songCountLabel;
		[SerializeField] private TMP_Text _gemsCountLabel;
		[SerializeField] private TMP_Text _boxCountLabel;
		[SerializeField] private TMP_Text _oldPriceLabel;

		public override string Type => ProductType.WelcomeBundle;

		protected override void FillUi()
		{
			var currentProduct = (WelcomeBundleProduct) _product;
			_titleLabel.text = currentProduct.Prooperty.Title;
			SetSongCount(currentProduct.Prooperty.SongCount);
			SetOldPrice(currentProduct.Prooperty.Price.OldValue);
			SetSongCount(currentProduct.Prooperty.SongCount);
			SetGemsCount(currentProduct.Prooperty.GemsCount);
			SetBoxCount(currentProduct.Prooperty.BoxCount);
		}

		private void SetSongCount(int count) =>
			_songCountLabel.text = $"<sprite=9 color=#6AFF92>{count} song";

		private void SetGemsCount(int count) =>
			_gemsCountLabel.text = $"<sprite=7 color=#B950FF>{count} gems";

		private void SetBoxCount(int count) =>
			_boxCountLabel.text = $"<sprite=8 color=#6A9CFF>{count} box";

		private void SetOldPrice(float priceValue) =>
				_oldPriceLabel.text = string.Format("<s>${0:f2}</s>", priceValue);
	}
}
