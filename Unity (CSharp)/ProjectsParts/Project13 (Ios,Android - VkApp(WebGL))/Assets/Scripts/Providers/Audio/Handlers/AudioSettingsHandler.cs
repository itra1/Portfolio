using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.Audio.Settings;

namespace Game.Providers.Audio.Handlers
{
	public class AudioSettingsHandler
	{
		private AudioProviderSettings _settings;
		private AudioProvider _audioProvider;
		private CancellationTokenSource _saveCTS;

		public AudioSettingsHandler(AudioProviderSettings _audioSettings, AudioProvider audioProvider)
		{
			_settings = _audioSettings;
			_audioProvider = audioProvider;
			_ = Confirm();
		}

		private async UniTask Confirm()
		{
			await UniTask.WaitUntil(() => _audioProvider.IsLoaded);
			SetSound(_audioProvider.Save.Value.Sound);
		}

		public bool CurrentSound()
		{
			return _settings.MainAudioGroup.audioMixer.GetFloat("Volume", out var volume) ? volume >= 0 : true;
		}

		public void SetSound(bool isOn)
		{
			_audioProvider.Save.Value.Sound = isOn;
			_ = _settings.MainAudioGroup.audioMixer.SetFloat("Volume", (isOn ? 0f : -80f));
			Save().Forget();
		}

		private async UniTask Save()
		{
			if (_saveCTS != null)
			{
				if (!_saveCTS.IsCancellationRequested)
					_saveCTS.Cancel();
				_saveCTS = null;
			}
			_saveCTS = new();

			try
			{
				await UniTask.Delay(300, cancellationToken: _saveCTS.Token);
				_ = _audioProvider.Save.Save();
			}
			catch (OperationCanceledException)
			{

			}
			finally
			{
				if (_saveCTS != null && _saveCTS.IsCancellationRequested)
					_saveCTS?.Dispose();
				_saveCTS = null;
			}
		}
	}
}
