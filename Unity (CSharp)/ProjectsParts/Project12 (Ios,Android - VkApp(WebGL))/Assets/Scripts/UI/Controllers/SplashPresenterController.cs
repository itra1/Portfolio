using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Splash, WindowPresenterNames.Splash)]
	public class SplashPresenterController : WindowPresenterController<SplashPresenter>
	{
		public override bool AddInNavigationStack => false;

	}
}
