using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Profile)]
	public class ProfileWindowPresenterController : WindowPresenterController<ProfileWindowPresenter>
	{
	}
}
