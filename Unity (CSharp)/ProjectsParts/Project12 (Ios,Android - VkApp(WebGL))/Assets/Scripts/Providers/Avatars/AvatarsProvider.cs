using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Avatars.Base;
using Game.Scripts.Providers.Avatars.Settings;
using UnityEngine;

namespace Game.Scripts.Providers.Avatars
{
	public class AvatarsProvider : IAvatarsProvider
	{
		[SerializeField] private List<AvatarSource> _avatars = new();

		private AvatarsSettings _settings;

		public List<AvatarSource> Avatars => _avatars;
		public List<AvatarSource> MaleAvatars => _avatars.FindAll(x => x.IsMale);
		public List<AvatarSource> FemaleAvatars => _avatars.FindAll(x => !x.IsMale);

		public AvatarsProvider(AvatarsSettings settings)
		{
			_settings = settings;
			_avatars.AddRange(_settings.Avatars);
		}

		public string RandomAvatarUuid() => _settings.Avatars[UnityEngine.Random.Range(0, _settings.Avatars.Count)].Uuid;
		public Sprite GetAvatar(string uuid) => Avatars.Find(x => x.Uuid == uuid).Avatar50Round10;
		public AvatarSource GetRandom() => _avatars[UnityEngine.Random.Range(0, _avatars.Count)];
		public async UniTask StartAppLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_avatars.AddRange(_settings.Avatars);
			await UniTask.Yield();
		}
	}
}
