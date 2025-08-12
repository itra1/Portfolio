using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Components.Shop;
using Core.Engine.Signals;
using Core.Engine.Utils;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Core.Engine.uGUI.Screens
{
	[PrefabName(ScreenTypes.Shop)]
	public class ShopScreen :Screen
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private ShopItem _shopItemPrefab;
		[SerializeField] private float _itemDistance = 5;

		private SignalBus _signalBus;
		private IShopProvider _shopProvider;
		private PrefabPool<ShopItem> _shopPool;

		[Inject]
		public void Initiate(SignalBus signalbus, IShopProvider shopProvider)
		{
			_signalBus = signalbus;
			_shopProvider = shopProvider;

			_shopPool = new(_shopItemPrefab, _scrollRect.content);
			_shopItemPrefab.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			SpawnItems();
		}

		private void SpawnItems()
		{
			_shopPool.HideAll();

			for (var i = 0; i < _shopProvider.Items.Count; i++)
			{
				var item = _shopProvider.Items[i];
				var elem = _shopPool.GetItem();
				elem.Set(item);
				elem.RT.anchoredPosition = new(elem.RT.anchoredPosition.x, -(i * (elem.RT.rect.height + _itemDistance)));
				elem.gameObject.SetActive(true);
			}
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, _shopProvider.Items.Count * (_shopItemPrefab.RT.rect.height + _itemDistance));
		}

		public void BackButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.FirstMenuOpen });
		}

	}
}
