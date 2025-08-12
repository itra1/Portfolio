using Cysharp.Threading.Tasks;
using Game.Scripts.Controllers.Sessions.Common;
using Game.Scripts.Helpers;
using Game.Scripts.Providers.Profiles.Handlers;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Popup, WindowPresenterNames.Develop)]
	public class DevelopPresenterController : WindowPresenterController<DevelopPresenter>
	{
		private IProfileStarsHandler _starsHandler;
		private IProfileLevelHandler _levelHandler;
		private ISession _session;
		private IPlatformHelper _appActions;

		public override bool AddInNavigationStack => false;

		[Inject]
		private void Build(
			IProfileStarsHandler starsHandler,
			IProfileLevelHandler levelHandler,
			ISession session,
			IPlatformHelper appActions
		)
		{
			_starsHandler = starsHandler;
			_levelHandler = levelHandler;
			_session = session;
			_appActions = appActions;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();
			Presenter.OnCloseEvent.AddListener(CloseButtonTouch);
			Presenter.OnAddOneStarEvent.AddListener(AddOneStarButtonTouchListener);
			Presenter.OnAddOneLevelEvent.AddListener(AddOneLevelButtonTouchListener);
			Presenter.OnSceneVisibleToggleEvent.AddListener(SceneVisibleToggleListener);
			Presenter.OnGameMissToggleEvent.AddListener(GameMissToggleListener);
			Presenter.OnTapVisibleToggleEvent.AddListener(TapVisibleToggleListener);
			Presenter.OnClearProgressButtonEvent.AddListener(ClearProgressListener);
			Presenter.OnGameStaticToggleEvent.AddListener(GameStatisticToggleListener);

			Presenter.SceneVisibleMoveSet(_session.SceneVisibleMode);
			Presenter.GameMissMoveSet(_session.GameMissMode);
			Presenter.GameStatisticSet(_session.GameStatistic);
		}

		private void GameStatisticToggleListener()
		{
			_session.GameStatisticSet(!_session.GameStatistic);
			Presenter.GameStatisticSet(_session.GameStatistic);
		}

		private void ClearProgressListener()
		{
			_appActions.ClearApplicationProgress();
		}

		private void TapVisibleToggleListener()
		{
			_session.TapVisible = !_session.TapVisible;
			Presenter.TapVisibleSet(_session.TapVisible);
		}

		public override async UniTask<bool> Open()
		{
			_ = await base.Open();
			Presenter.SceneVisibleMoveSet(_session.SceneVisibleMode);
			Presenter.GameMissMoveSet(_session.GameMissMode);

			return true;
		}

		private void SceneVisibleToggleListener()
		{
			_session.SceneVisibleToggle();
			Presenter.SceneVisibleMoveSet(_session.SceneVisibleMode);
		}

		private void GameMissToggleListener()
		{
			_session.GameMissToggle();
			Presenter.GameMissMoveSet(_session.GameMissMode);
		}

		private void AddOneLevelButtonTouchListener()
		{
			_levelHandler.AddOneLevel();
		}

		private void CloseButtonTouch()
		{
			_ = Close();
		}

		private void AddOneStarButtonTouchListener()
		{
			_starsHandler.AddStars(1);
		}
	}
}
