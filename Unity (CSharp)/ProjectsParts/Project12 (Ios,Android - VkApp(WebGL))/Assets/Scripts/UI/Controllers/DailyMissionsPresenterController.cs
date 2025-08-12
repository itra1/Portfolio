using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.DailyMissions;
using Game.Scripts.Providers.DailyMissions.Common;
using Game.Scripts.UI.Attributes;
using Game.Scripts.UI.Base;
using Game.Scripts.UI.Controllers.Base;
using Game.Scripts.UI.Presenters;
using Zenject;

namespace Game.Scripts.UI.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.DailyMission)]
	public class DailyMissionsPresenterController : WindowPresenterController<DailyMissionsPresenter>
	{
		private IDailyMissionsProvider _provider;
		public override bool AddInNavigationStack => true;

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();
			Presenter.OnBackEvent.AddListener(() => _ = OnBackEventListener());
			Presenter.OnRewardEvent.AddListener((mission) => _provider?.Reward(mission));
		}

		private async UniTask OnBackEventListener()
		{
			_ = await Close();
		}

		[Inject]
		public void Build(IDailyMissionsProvider provider)
		{
			_provider = provider;
		}

		public override async UniTask<bool> Open()
		{
			if (Presenter == null)
				await LoadPresenter();

			Presenter.SetMissions(_provider.ActiveMissions);

			if (!await base.Open())
				return false;

			_provider.OnMissionChangeEvent.AddListener(OnMissionChangeEventListener);

			return true;
		}

		public override async UniTask<bool> Close()
		{
			if (!await base.Close())
				return false;

			_provider.OnMissionChangeEvent.RemoveListener(OnMissionChangeEventListener);

			return true;
		}

		private void OnMissionChangeEventListener(MissionEventData _)
		{
			Presenter.FillData();
		}
	}
}
