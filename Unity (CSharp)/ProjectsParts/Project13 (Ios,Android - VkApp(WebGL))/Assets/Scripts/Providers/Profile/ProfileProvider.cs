using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Game.Settings;
using Game.Providers.Avatars;
using Game.Providers.Nicknames;
using Game.Providers.Profile.Common;
using Game.Providers.Profile.Models;
using Game.Providers.Profile.Save;
using Game.Providers.Profile.Settings;
using Game.Providers.Profile.Signals;
using Game.Providers.Saves;
using UnityEngine;
using Zenject;

namespace Game.Providers.Profile
{
	public class ProfileProvider : IProfileProvider
	{
		private ISaveProvider _saveGameProvider;
		private ProfileProviderSave _save;
		private ProfileModel _profile;
		private IAvatarsProvider _avatarsProvider;
		private SignalBus _signalBus;
		private INicknamesProvider _nicknamesProvider;
		private ProfileSettings _profileSettings;
		private GameSettings _gameSettings;
		private List<PlayerLevel> _levels = new();

		public bool IsLoaded { get; private set; }
		public List<PlayerLevel> Levels => _levels;
		public bool IsMaxLevel => CurrentLevel == NextLevel;
		public int CurrentLevel => _profile.Level;
		public int NextLevel => CurrentLevel + 1 >= _levels.Count ? CurrentLevel : CurrentLevel + 1;
		public int CurrentExpirience => (int) _profile.Experience;
		public int CurrentExperienceInLevel => CurrentExpirience - _levels[CurrentLevel].Experience;
		public int ExperienceInLevel => Levels[Mathf.Max(CurrentLevel, NextLevel)].Experience - Levels[CurrentLevel].Experience;
		public int ExperienceToNextLevel => Levels[Mathf.Max(CurrentLevel, NextLevel)].Experience;
		public bool ExistsRewardReady => Levels.Exists(x => x.RewardReady && x.Index != 0);
		public List<ProfileWeapon> Weapons
		{
			get => _profile.Weapons;
			set
			{
				_profile.Weapons = value;
				Save();
			}
		}

		public bool WelcomeShow
		{
			get => _profile.WelcomeShow;
			set
			{
				_profile.WelcomeShow = value;
				Save();
			}
		}

		public float Coins
		{
			get => _profile.Coins;
			set
			{
				_profile.Coins = value;
				Save();
			}
		}
		public float Dollar
		{
			get => _profile.Dollar;
			set
			{
				_profile.Dollar = value;
				Save();
			}
		}
		public float Experience
		{
			get => _profile.Experience;
			set
			{
				_profile.Experience = value;
				Save();
			}
		}
		public int Level
		{
			get => _profile.Level;
			set
			{
				_profile.Level = value;
				Save();
			}
		}

		public List<int> LevelsRewardsGet
		{
			get => _profile.LevelsRewardsGet;
			set
			{
				_profile.LevelsRewardsGet = value;
				Save();
			}
		}

		public string Avatar
		{
			get => _profile.Avatar;
			set
			{
				_profile.Avatar = value;
				Save();
			}
		}

		public string Name => _profile.Name;

		public ProfileProvider(
			SignalBus signalbus,
			ISaveProvider saveGameProvider,
			IAvatarsProvider avatarProvider,
			INicknamesProvider nicknamesProvider,
			ProfileSettings profileSettings,
			GameSettings gameSettings
		)
		{
			_saveGameProvider = saveGameProvider;
			_avatarsProvider = avatarProvider;
			_nicknamesProvider = nicknamesProvider;
			_signalBus = signalbus;
			_profileSettings = profileSettings;
			_gameSettings = gameSettings;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_save = _saveGameProvider.GetProperty<ProfileProviderSave>();
			_profile = _save.Value;

			MakeLevels();
			if (string.IsNullOrEmpty(_profile.Avatar))
			{
				SetStartValue();
			}
			if (!_avatarsProvider.ExistsAvatar(_profile.Avatar))
			{
				SetRandomAvatar();
				Save();
			}
			await UniTask.Yield();
		}

		private void MakeLevels()
		{
			for (var i = 0; i < _profileSettings.LevelRewards.Count; i++)
			{
				var pl = new PlayerLevel(this, i, _profileSettings.LevelRewards[i]);
				_levels.Add(pl);
			}
		}

		private void SetStartValue()
		{
			SetStartNickname();
			SetRandomAvatar();
			_ = _saveGameProvider.Save();

			for (var i = 0; i < _gameSettings.RegisterRewards.Rewards.Length; i++)
			{
				_signalBus.Fire(new ResourceAddSignal(_gameSettings.RegisterRewards.Rewards[i].Type, _gameSettings.RegisterRewards.Rewards[i].Value, null));
			}
		}
		private void SetStartNickname()
		{
			_profile.Name = _nicknamesProvider.GetRandom();
			Save();
		}

		private void SetRandomAvatar()
		{
			_profile.Avatar = _avatarsProvider.Avatars.First().Key;
		}
		public void SetNickname(string name)
		{
			_profile.Name = name;
			Save();
		}

		public void SetAvatar(string name)
		{
			_profile.Avatar = name;
			Save();
			_signalBus.Fire<AvatarChangeSignal>();
		}

		public void AddWin()
		{
			_profile.Wins += 1;
			Save();
			_signalBus.Fire<ProfileGameChangeSignal>();
		}

		public void AddDefeat()
		{
			_profile.Defeat += 1;
			Save();
			_signalBus.Fire<ProfileGameChangeSignal>();
		}

		public bool IsReceivedReward(int index)
		{
			return _profile.LevelsRewardsGet.Contains(index);
		}

		public void Save()
		{
			_ = _saveGameProvider.Save();
		}
	}
}
