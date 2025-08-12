using Cysharp.Threading.Tasks;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui;
using Game.Providers.Ui.Controllers;
using Zenject;

namespace Game.Game.Handlers
{
	public class OpenAddDollarHandler
	{

		private SignalBus _signals;
		private IUiProvider _uiProvider;

		public OpenAddDollarHandler(SignalBus signalBus, IUiProvider uiProvider)
		{
			_signals = signalBus;
			_uiProvider = uiProvider;
			_signals.Subscribe<OpenAddDollarDialogSignal>(() =>
			{
				OpenDialoAsync().Forget();
			});
		}
		public void OpenAddDollarDialog()
		{
			OpenDialoAsync().Forget();
		}
		private async UniTask OpenDialoAsync()
		{

			var developController = _uiProvider.GetController<DevelopWindowPresenterController>();
			//var popup = _popupProvider.GetPopup(PopupsNames.AddBucks);
			//var popup = (DevelopPopup) _uiProvider.GetPopup(PopupsNames.Develop);
			_ = await developController.Show(null);
			developController.Presenter.SetDefault();
		}
	}
}
