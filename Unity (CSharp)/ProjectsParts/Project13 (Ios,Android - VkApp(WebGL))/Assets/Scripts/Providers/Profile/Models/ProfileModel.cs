using System;
using System.Collections.Generic;
using Game.Providers.Profile.Common;

namespace Game.Providers.Profile.Models
{
	public class ProfileModel
	{
		public string Name;
		public bool WelcomeShow = false;
		public float Coins = 0;
		public float Dollar = 0;
		public float Experience = 0;
		public int Wins = 0;
		public int Defeat = 0;
		public string Avatar;
		public int Level = 0;
		public List<int> LevelsRewardsGet = new();
		public DateTime WithdrawalSend = DateTime.MinValue;
		public List<string> TournamentsGames = new();
		public List<ProfileWeapon> Weapons = new();
		public string DollarAsString => Math.Round(Dollar, 1).ToString().Replace(",", ".");
	}
}
