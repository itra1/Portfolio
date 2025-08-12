using Game.Scripts.Managers.Base;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.GamePause)]
	public class GamePausePresenterController : WindowPresenterController<GamePausePresenter>
	{
		private IPauseHandler _pauseHelper;
		public override bool AddInNavigationStack => true;

		[Inject]
		private void Build(IPauseHandler pauseHelper)
		{
			_pauseHelper = pauseHelper;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnCloseEvent.AddListener(CloseEvent);
		}

		private void CloseEvent()
		{
			_pauseHelper.UnPause();
		}
	}
}
