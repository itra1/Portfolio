using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Products;
using Game.Scripts.Providers.Shop.Settings;
using Game.Scripts.UI.Presenters.Base;
using Game.Scripts.UI.Shop;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI.Presenters
{
	public class ShopPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnBackEvent = new();
		[HideInInspector] public UnityEvent<IProduct> OnBuyProduct = new();

		[SerializeField] private Button _backButton;
		[SerializeField] private GameObject _packListParentPrefab;
		[SerializeField] private RectTransform _productContent;
		[SerializeField] private RectTransform _pack1Content;
		[SerializeField] private RectTransform _pack2Content;

		private List<ProductButtonUi> _buttons = new();
		private List<GameObject> _packPrefabsList = new();

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_backButton.onClick.AddListener(BackButtonTouch);

			return true;
		}


		private void BackButtonTouch()
		{
			OnBackEvent?.Invoke();
		}

		protected override void PositionContent()
		{
			_positionTransform.anchoredPosition = Vector2.zero;
			_positionTransform.sizeDelta = Vector2.zero;
		}

		public void VisibleButtons(List<IProduct> shopProduct, ShopSettings shopSettings)
		{
			foreach (var item in _buttons)
				item.gameObject.SetActive(false);
			foreach (var item in _packPrefabsList)
				item.gameObject.SetActive(false);

			var packProducts = shopProduct.FindAll(x => x.Type == ProductType.SongsPack);

			var welcomeBundleButtonPrefab = shopSettings.ShopButtons.Find(x => x.Type == ProductType.WelcomeBundle);
			var songsPackButtonPrefab = shopSettings.ShopButtons.Find(x => x.Type == ProductType.SongsPack);
			var premiumButtonPrefab = shopSettings.ShopButtons.Find(x => x.Type == ProductType.PremiumSubscribe);

			//////////////////////
			var welcomeProduct = shopProduct.Find(x => x.Type == ProductType.WelcomeBundle);
			ProductButtonUi welcomeProductInstance = _buttons.Find(x => x.Type == ProductType.WelcomeBundle && !x.gameObject.activeSelf);
			if (welcomeProductInstance == null)
			{
				welcomeProductInstance = Instantiate(welcomeBundleButtonPrefab, _productContent);
				_buttons.Add(welcomeProductInstance);
			}
			welcomeProductInstance.gameObject.SetActive(true);
			welcomeProductInstance.SetProduct(welcomeProduct);
			welcomeProductInstance.OnBuyTouchEvent = OnBuyProductEmit;

			//////////////////////

			if (packProducts.Count > 0)
			{
				var packParent = _packPrefabsList.Find(x => !x.gameObject.activeSelf);
				if (packParent == null)
				{
					packParent = Instantiate(_packListParentPrefab, _productContent);
					_packPrefabsList.Add(packParent);
				}
				packParent.SetActive(true);

				for (int i = 0; i < Mathf.Min(3, packProducts.Count); i++)
				{
					ProductButtonUi packItemInstance = _buttons.Find(x => x.Type == ProductType.SongsPack && !x.gameObject.activeSelf);
					if (packItemInstance == null)
					{
						packItemInstance = Instantiate(songsPackButtonPrefab, packParent.transform);
						_buttons.Add(packItemInstance);
					}
					packItemInstance.gameObject.SetActive(true);
					packItemInstance.SetProduct(packProducts[i]);
					packItemInstance.OnBuyTouchEvent = OnBuyProductEmit;
				}
			}

			//////////////////////

			var premiumProduct = shopProduct.Find(x => x.Type == ProductType.PremiumSubscribe);
			//if (premiumProduct.ReadyShow)
			//{
			ProductButtonUi premiumButtonInstance = _buttons.Find(x => x.Type == ProductType.PremiumSubscribe && !x.gameObject.activeSelf);
			if (premiumButtonInstance == null)
			{
				premiumButtonInstance = Instantiate(premiumButtonPrefab, _productContent);
				_buttons.Add(premiumButtonInstance);
			}
			premiumButtonInstance.gameObject.SetActive(true);
			premiumButtonInstance.SetProduct(premiumProduct);
			premiumButtonInstance.OnBuyTouchEvent = OnBuyProductEmit;
			//}

			//////////////////////

			if (packProducts.Count > 3)
			{
				var packParent = _packPrefabsList.Find(x => !x.gameObject.activeSelf);
				if (packParent == null)
				{
					packParent = Instantiate(_packListParentPrefab, _productContent);
					_packPrefabsList.Add(packParent);
				}
				packParent.SetActive(true);

				for (int i = 3; i < packProducts.Count; i++)
				{
					ProductButtonUi packItemInstance = _buttons.Find(x => x.Type == ProductType.SongsPack && !x.gameObject.activeSelf);
					if (packItemInstance == null)
					{
						packItemInstance = Instantiate(songsPackButtonPrefab, packParent.transform);
						_buttons.Add(packItemInstance);
					}
					packItemInstance.gameObject.SetActive(true);
					packItemInstance.SetProduct(packProducts[i]);
					packItemInstance.OnBuyTouchEvent = OnBuyProductEmit;
				}
			}
		}

		private void OnBuyProductEmit(IProduct product)
		{

		}
	}
}
