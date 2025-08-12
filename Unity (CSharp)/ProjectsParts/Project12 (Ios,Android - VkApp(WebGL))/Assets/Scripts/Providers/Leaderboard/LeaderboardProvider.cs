using System.Collections.Generic;
using Game.Scripts.Providers.Leaderboard.Base;
using Game.Scripts.Providers.Leaderboard.Settings;

namespace Game.Scripts.Providers.Leaderboard
{
	public class LeaderboardProvider : ILeaderboardProvider
	{
		private readonly LeaderboardSettings _settings;

		public List<LeaderboardItem> LeaderboardList => _settings.Leaderboard;

		public LeaderboardProvider(LeaderboardSettings settings)
		{
			_settings = settings;
		}
	}
}
