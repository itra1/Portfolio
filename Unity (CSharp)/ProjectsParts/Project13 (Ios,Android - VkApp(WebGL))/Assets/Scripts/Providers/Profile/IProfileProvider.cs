using System.Collections.Generic;
using Game.Base.AppLaoder;
using Game.Providers.Profile.Common;

namespace Game.Providers.Profile
{
	public interface IProfileProvider : IAppLoaderElement
	{
		string Name { get; }
		int CurrentLevel { get; }
		float Coins { get; set; }
		float Dollar { get; set; }
		float Experience { get; set; }
		int Level { get; set; }
		bool IsMaxLevel { get; }
		int CurrentExpirience { get; }
		int ExperienceToNextLevel { get; }
		int CurrentExperienceInLevel { get; }
		int ExperienceInLevel { get; }
		int NextLevel { get; }
		List<PlayerLevel> Levels { get; }
		bool ExistsRewardReady { get; }
		List<ProfileWeapon> Weapons { get; set; }
		bool WelcomeShow { get; set; }
		List<int> LevelsRewardsGet { get; set; }
		string Avatar { get; set; }

		void AddDefeat();
		void AddWin();
		bool IsReceivedReward(int index);
		void SetAvatar(string name);
		void SetNickname(string name);
		void Save();
	}
}