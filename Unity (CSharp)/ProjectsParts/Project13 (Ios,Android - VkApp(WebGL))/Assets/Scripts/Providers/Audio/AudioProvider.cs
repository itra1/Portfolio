using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.Audio.Save;
using Game.Providers.Saves;
using UnityEngine;

namespace Game.Providers.Audio
{
	public class AudioProvider : IAudioProvider
	{
		private ISaveProvider _saveProvider;
		[SerializeField] private AudioProviderSave _save;

		public AudioProviderSave Save => _save;
		public bool IsLoaded { get; private set; }

		public AudioProvider(ISaveProvider saveProvider)
		{
			_saveProvider = saveProvider;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_save = _saveProvider.GetProperty<AudioProviderSave>();
			IsLoaded = true;
			await UniTask.Yield();
		}
	}
}
