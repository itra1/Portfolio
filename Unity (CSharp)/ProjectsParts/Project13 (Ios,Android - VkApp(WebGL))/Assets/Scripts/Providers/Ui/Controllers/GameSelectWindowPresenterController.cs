using Game.Providers.Battles.Helpers;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;
using Zenject;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.GameSelect)]
	public class GameSelectWindowPresenterController : WindowPresenterController<GameSelectWindowPresenter>
	{
		private IBattleHelper _battleHelper;

		[Inject]
		private void Build(IBattleHelper battleHelper)
		{
			_battleHelper = battleHelper;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();
			Presenter.OnSoloSelect.AddListener(SelectSolo);
			Presenter.OnDuelSelect.AddListener(SelectDuel);
		}

		public void SelectSolo()
		{
			_ = _uiProvider.GetController<DevelopWindowPresenterController>().Show(this);
		}

		public void SelectDuel() => _battleHelper.RunDuel();
	}
}
