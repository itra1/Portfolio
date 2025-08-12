using Cysharp.Threading.Tasks;
using Game.Providers.Battles.Helpers;
using Game.Providers.Timers.Common;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Base;
using Game.Providers.Ui.Controllers.Base;
using Game.Providers.Ui.Presenters;
using Zenject;

namespace Game.Providers.Ui.Controllers
{
	[UiController(WindowPresenterType.Window, WindowPresenterNames.LookingOpponent)]
	public class LookingOpponentWindowPresenterController : WindowPresenterController<LookingOpponentWindowPresenter>
	{
		private IBattleHelper _battleHelper;

		[Inject]
		private void Bind(IBattleHelper battleHelper)
		{
			_battleHelper = battleHelper;
		}

		public async override UniTask<bool> Show(IWindowPresenterController source)
		{
			if (!await base.Show(source))
				return false;

			//_ = StartProcess();

			return true;
		}

		public void SetTimer(ITimer timer) => Presenter.SetTimer(timer);

		//private async UniTask StartProcess()
		//{
		//	await UniTask.Delay(100);

		//	_ = _battleHelper.RunDuel();
		//}
	}
}
