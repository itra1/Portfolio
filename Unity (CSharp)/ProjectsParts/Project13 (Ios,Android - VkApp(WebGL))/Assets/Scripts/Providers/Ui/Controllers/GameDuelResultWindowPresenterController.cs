using Cysharp.Threading.Tasks;
using Game.Providers.Battles;
using Game.Providers.Battles.Components;
using Game.Providers.Battles.Helpers;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;
using Zenject;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.GameDuelResult)]
	public class GameDuelResultWindowPresenterController : WindowPresenterController<GameDuelResultWindowPresenter>
	{
		private IUiProvider _uiPresenter;
		private IBattleHelper _battleHelper;
		private IBattleProvider _battleProvider;
		private BattleDuelState _state;

		[Inject]
		private void Build(IUiProvider uiPresenter, IBattleHelper battleHelper, IBattleProvider battleProvider)
		{
			_uiPresenter = uiPresenter;
			_battleHelper = battleHelper;
			_battleProvider = battleProvider;
		}

		public void SetState(BattleDuelState state)
		{
			_state = state;
		}

		public override async UniTask<bool> Show(IWindowPresenterController source)
		{
			if (!await base.Show(source))
				return false;

			Presenter.SetState(_state);

			return true;
		}

		protected override void AfterCreateWindow()
		{
			base.AfterCreateWindow();

			Presenter.OnExit.AddListener(ExitButtonTouch);
			Presenter.OnRematch.AddListener(RematchButtonTouch);
		}

		private void RematchButtonTouch()
		{
			_ = Hide();

			_battleProvider.RunDuel();
		}

		private void ExitButtonTouch()
		{
			_ = Hide();

			var controller = _uiProvider.GetController<HomeWindowPresenterController>();

			_ = controller.Show(null);
		}
	}
}
