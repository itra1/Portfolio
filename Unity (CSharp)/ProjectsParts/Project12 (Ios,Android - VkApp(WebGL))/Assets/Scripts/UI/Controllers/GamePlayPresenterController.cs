using Game.Scripts.Managers.Base;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.GamePlay)]
	public class GamePlayPresenterController : WindowPresenterController<GamePlayPresenter>
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

			Presenter.PauseTouchEvent.AddListener(PauseSet);
		}

		private void PauseSet()
		{
			//if (_pauseHelper.IsPaused)
			//	_pauseHelper.UnPause();
			//else
			_pauseHelper.Pause();
		}
	}
}
