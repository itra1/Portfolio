using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Splash, WindowPresenterNames.Loader)]
	public class LoaderWindowPresenterController : WindowPresenterController<LoaderWindowPresenter>
	{
	}
}
