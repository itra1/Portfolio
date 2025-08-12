using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Shop)]
	public class ShopWindowPresenterController : WindowPresenterController<ShopWindowPresenter>
	{
		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnBackTouch.AddListener(BackButtonTouch);
		}

		private void BackButtonTouch()
		{
			_ = ParentOpen();
			_ = Hide();
		}
	}
}
