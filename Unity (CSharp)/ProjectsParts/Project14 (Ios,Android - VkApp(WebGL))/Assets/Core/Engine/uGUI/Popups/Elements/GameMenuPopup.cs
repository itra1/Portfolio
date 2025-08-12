using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Components.SaveGame;
using Core.Engine.Signals;
using Core.Engine.uGUI.Elements;
using UnityEngine;
using Zenject;

namespace Core.Engine.uGUI.Popups
{
	/// <summary>
	/// Попап меню игры
	/// </summary>
	[PrefabName(PopupTypes.GameMenu)]
	public class GameMenuPopup :Popup
	{
		[SerializeField] private ToggleElement _soundToggle;
		[SerializeField] private ToggleElement _vibroToggle;

		private IAudioProvider _audioProvider;
		private SignalBus _signalBus;

		private VibrationEnable _vibrationEnableSG;

		[Inject]
		public void Initialize(ISaveGameProvider sessonProvider
		,SignalBus signalBud
		,IAudioProvider audioProvider
		)
		{
			_audioProvider = audioProvider;
			_signalBus = signalBud;
			_vibrationEnableSG = (VibrationEnable)sessonProvider.GetProperty<VibrationEnable>();
		}

		private void OnEnable()
		{
			_soundToggle.OnChange.RemoveAllListeners();
			_vibroToggle.OnChange.RemoveAllListeners();

			_soundToggle.IsActive = _audioProvider.SoundIsEnable;

			if (_vibrationEnableSG != null)
				_vibroToggle.IsActive = _vibrationEnableSG.Value;

			_soundToggle.OnChange.AddListener((value) =>
			{
				PlayAudio.PlaySound("click");
				_audioProvider.SoundIsEnable = value;
			});
			_vibroToggle.OnChange.AddListener(OnVibroChange);
		}

		private void OnVibroChange(bool value)
		{
			PlayAudio.PlaySound("click");
			_vibrationEnableSG.Value = value;
		}

		public void BackButtonTouch()
		{
			PlayAudio.PlaySound("click");
			Hide();
		}

		public void ExitButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire<GameExitPlaySignal>();
			Hide();
		}
	}
}
