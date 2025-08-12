using Cysharp.Threading.Tasks;
using Game.Game.Signals;
using Game.Providers.Ui.Popups;
using Game.Providers.Ui.Popups.Common;
using Zenject;

namespace Game.Game.Handlers {
	internal class OpenSettingsHandler {

		private SignalBus _signals;
		private IPopupsProvider _popupProvider;

		public OpenSettingsHandler(SignalBus signalBus, IPopupsProvider popupsProvider) {
			_signals = signalBus;
			_popupProvider = popupsProvider;
			_signals.Subscribe<OpenSettingsSignal>(OnOpenSettingsSignal);
		}

		private void OnOpenSettingsSignal() {
			OnOpenSettingsSignalAsync().Forget();
		}
		private async UniTask OnOpenSettingsSignalAsync() {

			var popup = _popupProvider.GetPopup(PopupsNames.Settiings);
			await popup.Show();
		}

	}
}
