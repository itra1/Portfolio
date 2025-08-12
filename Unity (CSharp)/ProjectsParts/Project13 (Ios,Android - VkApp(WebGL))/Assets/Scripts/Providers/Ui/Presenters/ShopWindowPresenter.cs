using Cysharp.Threading.Tasks;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Presenters.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Providers.Ui.Presenters
{
	public class ShopWindowPresenter : WindowPresenter
	{
		[HideInInspector] public UnityEvent OnBackTouch;

		[SerializeField] private Button _backButton;
		[SerializeField] private ShopProductList _productList;

		public override async UniTask<bool> Initialize()
		{
			if (!await base.Initialize())
				return false;

			_backButton.onClick.RemoveAllListeners();
			_backButton.onClick.AddListener(BackButtonTouch);

			return true;
		}

		public override void Show()
		{
			base.Show();
			_productList.Show();
		}

		public override void Hide()
		{
			base.Hide();
			_productList.Hide();
		}

		private void BackButtonTouch() => OnBackTouch?.Invoke();
	}
}
