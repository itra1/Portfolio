using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.RewardFake)]
	public class RewardFakePresenterController : WindowPresenterController<RewardFakePresenter>
	{
		public override bool AddInNavigationStack => true;
		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();
			Presenter.ClaimTouchEvent.AddListener(ClaimTouchEvent);
			Presenter.CloseEvent.AddListener(CloseEvent);
		}

		private void CloseEvent()
		{
			_ = Close();
		}

		private void ClaimTouchEvent()
		{
			UiNavigator.ClearStack();
			_ = UiNavigator.GetController<HomePresenterController>().Open();
		}
	}
}
