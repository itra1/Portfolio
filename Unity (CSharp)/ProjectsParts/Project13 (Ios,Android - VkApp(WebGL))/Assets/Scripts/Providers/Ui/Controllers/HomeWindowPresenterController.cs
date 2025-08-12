using Game.Providers.Battles;
using Game.Providers.Battles.Helpers;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;
using Zenject;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.Home)]
	public class HomeWindowPresenterController : WindowPresenterController<HomeWindowPresenter>
	{
		private IBattleHelper _tournamentHelper;
		private IBattleProvider _tournamentProvider;

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnPlayButton.AddListener(PlayButtonTouch);
			Presenter.OnShopButton.AddListener(ShopButtonTouch);
			Presenter.OnLeadersButton.AddListener(LeaderButtonTouch);
			Presenter.OnDailyMission.AddListener(DailyMissionTouch);
			Presenter.OnDailyBonus.AddListener(DailyBonusTouch);
			Presenter.OnWatchAds.AddListener(DailyButtonTouch);
			Presenter.OnBattle.AddListener(BattleTouch);
		}

		[Inject]
		public void Bould(IBattleHelper tournamentHelper, IBattleProvider tournamentProvider)
		{
			_tournamentHelper = tournamentHelper;
			_tournamentProvider = tournamentProvider;
		}

		private void BattleTouch()
		{
		}

		private void DailyButtonTouch()
		{
		}

		private void DailyBonusTouch()
		{
		}

		private void DailyMissionTouch()
		{
		}

		private void LeaderButtonTouch()
		{
		}

		private void ShopButtonTouch()
		{
			_ = Hide();
			_ = _uiProvider.GetController<ShopWindowPresenterController>().Show(this);
		}

		private void PlayButtonTouch()
		{
			_ = Hide();
			_ = _uiProvider.GetController<GameSelectWindowPresenterController>().Show(null);
		}
	}
}
