using Core.Engine.Components.SaveGame;
using UnityEngine;
using UnityEngine.Audio;

using Zenject;

namespace Core.Engine.Components.Audio
{
	public class AudioProvider : IAudioProvider
	{

		private SignalBus _signalBus;
		private SoundSaveGame _soundSaveGame;
		private ISoundSettings _soundSettings;

		private AudioMixer _soundMixer;
		private AudioMixerSnapshot _soundOnSnapshot;
		private AudioMixerSnapshot _soundOffSnapshot;
		public bool SoundIsEnable
		{
			get => _soundSaveGame.Value; 
			set
			{
				if (_soundSaveGame.Value == value) return;

				_soundSaveGame.Value = value;

				if (_soundSaveGame.Value)
					_soundOnSnapshot.TransitionTo(0);
				else
					_soundOffSnapshot.TransitionTo(0);

				_signalBus?.Fire(new SoundChangeSignal() { ISEnabled = _soundSaveGame.Value });
			}
		}

		public AudioProvider(SignalBus signalBus
		, ISaveGameProvider sessonProvider
		, ISoundSettings soundSettings)
		{
			_signalBus = signalBus;
			_soundSettings = soundSettings;
			_soundSaveGame = (SoundSaveGame)sessonProvider.GetProperty<SoundSaveGame>();

			LoadResources();
		}

		private void LoadResources()
		{
			_soundMixer = Resources.Load<AudioMixer>(_soundSettings.SoundSettingsFolder + "/Sound");

			if(_soundMixer == null){
				Debug.LogError("[AudioProvider] no exists soundMixer");
			}else
			{
				_soundOnSnapshot = _soundMixer.FindSnapshot("SoundOn");
				_soundOffSnapshot = _soundMixer.FindSnapshot("SoundOff");
			}
		}

		public void SoundEnableToggle()
		{
			SoundIsEnable = !SoundIsEnable;
		}


	}
}
