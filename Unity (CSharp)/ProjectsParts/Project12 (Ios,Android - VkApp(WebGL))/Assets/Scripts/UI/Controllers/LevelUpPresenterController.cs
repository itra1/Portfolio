using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.LevelUp)]
	public class LevelUpPresenterController : WindowPresenterController<LevelUpPresenter>
	{
		public override bool AddInNavigationStack => true;
		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnClaimTouch.AddListener(OnClaimTouchEvent);
		}

		private void OnClaimTouchEvent()
		{
			_ = Close();
		}

		public void SetLevel(int level)
		{
			Presenter.SetLevel(level);
		}
	}
}
