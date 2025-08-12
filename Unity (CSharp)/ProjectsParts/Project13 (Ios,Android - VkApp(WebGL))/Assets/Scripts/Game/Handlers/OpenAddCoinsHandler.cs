using Cysharp.Threading.Tasks;
using Game.Providers.Ui;
using Game.Providers.Ui.Controllers;
using Zenject;

namespace Game.Game.Handlers
{
	public class OpenAddCoinsHandler
	{
		private SignalBus _signals;
		private IUiProvider _uiProvider;

		public OpenAddCoinsHandler(SignalBus signalBus, IUiProvider uiProvider)
		{
			_signals = signalBus;
			_uiProvider = uiProvider;
		}

		public void OpenAddCoinsDialog()
		{
			OnOpenAddCoinsDialogSignalAsync().Forget();
		}
		private async UniTask OnOpenAddCoinsDialogSignalAsync()
		{
			var developController = _uiProvider.GetController<DevelopWindowPresenterController>();
			//var popup = _popupProvider.GetPopup(PopupsNames.AddCoins);
			//var popup = (DevelopPopup) _uiProvider.GetPopup(PopupsNames.Develop);
			_ = await developController.Show(null);
			developController.Presenter.SetDefault();
		}
	}
}
