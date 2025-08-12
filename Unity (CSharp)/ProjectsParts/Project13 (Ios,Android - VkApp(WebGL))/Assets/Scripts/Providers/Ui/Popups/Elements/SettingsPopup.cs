using Cysharp.Threading.Tasks;
using Game.Common.Attributes;
using Game.Game.Settings;
using Game.Providers.Audio.Handlers;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Popups.Elements
{
	[PrefabName(PopupsNames.Settiings)]
	public class SettingsPopup : Popup
	{
		[SerializeField] private ToggleElement _soundToggle;

		private AudioSettingsHandler _audioSettingsHandler;
		private GameSettings _gameSettings;

		[Inject]
		public void Constructor(AudioSettingsHandler audioHandler, GameSettings gameSettings)
		{
			_audioSettingsHandler = audioHandler;
			_gameSettings = gameSettings;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			_soundToggle.OnChange.RemoveAllListeners();

			_soundToggle.IsActive = _audioSettingsHandler.CurrentSound();

			_soundToggle.OnChange.AddListener((isOn) =>
			{
				_audioSettingsHandler.SetSound(isOn);
			});
		}

		public void CloseButtonTouch()
		{
			Hide().Forget();
		}

		public void PrivacyButtonTouch()
		{
			Application.OpenURL(_gameSettings.PrivacyUrl);
		}

		public void TermsOfSserviceButtonTtouch()
		{
			Application.OpenURL(_gameSettings.TermsUrl);
		}
	}
}
