using Game.Base;
using Game.Common.Attributes;
using Game.Game.Settings;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Popups.Elements {
	[PrefabName(PopupsNames.TutorialResult)]
	public class TutorialResultPopup : Popup {
		[SerializeField] private RectTransform _coinsParent;
		[SerializeField] private RectTransform _experienceParent;

		private SignalBus _signalBus;
		private GameSettings _gameSettings;

		[Inject]
		public void Constructor(SignalBus signalBus, GameSettings gameSettings) {
			_signalBus = signalBus;
			_gameSettings = gameSettings;
		}

		public void CloseButtonTouch() {
			_signalBus.Fire(new ResourceAddSignal(RewardTypes.Experience, 0, _experienceParent));
			_signalBus.Fire(new ResourceAddSignal(_gameSettings.TutorialReward.Type, _gameSettings.TutorialReward.Value, _coinsParent));
			_ = Hide();
		}
	}
}
