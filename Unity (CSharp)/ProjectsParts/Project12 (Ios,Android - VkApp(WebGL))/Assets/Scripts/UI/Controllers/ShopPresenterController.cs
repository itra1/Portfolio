using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Shop;
using Game.Scripts.Providers.Shop.Settings;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Shop)]
	public class ShopPresenterController : WindowPresenterController<ShopPresenter>
	{
		private IShopProvider _shopProvider;
		private ShopSettings _shopSettings;
		public override bool AddInNavigationStack => true;

		[Inject]
		private void Build(IShopProvider shopProvider, ShopSettings shopSettings)
		{
			_shopProvider = shopProvider;
			_shopSettings = shopSettings;
		}

		public override async UniTask<bool> Open()
		{
			if (Presenter == null)
				await LoadPresenter();

			Presenter.VisibleButtons(_shopProvider.ProductList, _shopSettings);

			if (!await base.Open())
				return false;

			return true;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnBackEvent.AddListener(() => _ = BackTouch());
		}

		private async UniTask BackTouch()
		{
			_ = await Close();
			//_ = UiNavigator.BackNavigation();
			//_ = Close();
			//_ = _sourceOpen.Open(null);
		}
	}
}
