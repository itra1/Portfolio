using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Reward)]
	public class RewardPresenterController : WindowPresenterController<RewardPresenter>
	{
		public override bool AddInNavigationStack => true;
	}
}
