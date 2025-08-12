using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Avatars;
using Game.Scripts.Providers.Profiles.Common;
using Game.Scripts.Providers.Profiles.Save;
using Game.Scripts.Providers.Saves;
using UnityEngine.Events;

namespace Game.Scripts.Providers.Profiles
{
	public class ProfileProvider : IProfileProvider
	{
		public UnityEvent<IProfile> OnProfileChange { get; } = new();

		private readonly ISaveProvider _saveProvider;
		private readonly IAvatarsProvider _avatarProvider;
		private IProfile _profileData;

		public IProfile Profile => _profileData;
		public bool IsFirstLogin => _profileData.FirstLogin == DateTime.MinValue;

		public ProfileProvider(
			ISaveProvider saveProvider,
			IAvatarsProvider avatarProvider
		)
		{
			_saveProvider = saveProvider;
			_avatarProvider = avatarProvider;
		}

		public async UniTask StartAppLoad(
			IProgress<float> OnProgress,
			CancellationToken cancellationToken
		)
		{
			_profileData = _saveProvider.GetProperty<ProfileSaveData>().Value;

			if (string.IsNullOrEmpty(_profileData.UserName))
				FirstInitializeData();
			await UniTask.Yield();
		}

		private void FirstInitializeData()
		{
			_profileData.FirstLogin = DateTime.MinValue;
			_profileData.UserName = "Player";
			_profileData.Points = 0;
			_profileData.Level = 0;
			_profileData.Balance = 0;
			_profileData.AvatarUuid = _avatarProvider.RandomAvatarUuid();
			Save();
		}

		public void FirstRun()
		{
			_profileData.FirstLogin = DateTime.Now;
			Save();
		}

		public void SetAvatar(string avatar)
		{
			_profileData.AvatarUuid = avatar;
			Save();
			OnProfileChange?.Invoke(_profileData);
		}

		private void Save()
		{
			_ = _saveProvider.Save();
		}
	}
}
