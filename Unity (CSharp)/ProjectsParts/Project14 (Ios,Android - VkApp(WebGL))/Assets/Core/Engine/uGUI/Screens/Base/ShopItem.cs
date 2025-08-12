
using Core.Engine.Components.Audio;
using Core.Engine.Components.Shop;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Core.Engine.uGUI.Screens
{
	public class ShopItem : Core.Engine.uGUI.Elements.ShopProductPanel
	{
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private TMP_Text _priceLabel;
		[SerializeField] private Image _icone;
		[SerializeField] private Button _buyButton;
		[SerializeField] private RectTransform _purchasedRect;


		protected override void Confirm()
		{
			if (_product == null) return;

			_icone.sprite = _product.Icone;
			_titleLabel.text = _product.Title;
			_priceLabel.text = _product.Price.ToString();
			_buyButton.gameObject.SetActive(!_product.IsAlreadyBuyed);
			_buyButton.interactable = _product.IsBuyReady;
			_purchasedRect.gameObject.SetActive(_product.IsAlreadyBuyed);

			_buyButton.onClick.RemoveAllListeners();
			_buyButton.onClick.AddListener(() =>
			{
				PlayAudio.PlaySound("click");
				Buy();
			});
		}


	}
}
