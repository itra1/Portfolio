using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.DailyMissions)]
	public class DailyMissionWindowPresenterController : WindowPresenterController<DailyMissionWindowPresenter>
	{
	}
}
