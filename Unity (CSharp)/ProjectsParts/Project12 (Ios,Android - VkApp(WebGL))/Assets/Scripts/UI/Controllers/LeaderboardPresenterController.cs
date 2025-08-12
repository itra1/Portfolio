using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Leaderboard;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Leaderboar)]
	public class LeaderboardPresenterController : WindowPresenterController<LeaderboardPresenter>
	{
		private ILeaderboardProvider _leaderboardProvider;
		public override bool AddInNavigationStack => true;

		[Inject]
		public void Build(ILeaderboardProvider leaderboardProvider)
		{
			_leaderboardProvider = leaderboardProvider;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();
			Presenter.OnBackEvent.AddListener(() => _ = BackEvent());
		}

		public override async UniTask<bool> Open()
		{
			if (Presenter == null)
				await LoadPresenter();

			Presenter.SetData(_leaderboardProvider.LeaderboardList);
			if (!await base.Open())
				return false;

			return true;
		}

		private async UniTask BackEvent()
		{
			_ = await Close();
			//_ = UiNavigator.BackNavigation();
			//_ = _sourceOpen?.Open(false);
			//_ = Close();
		}
	}
}
