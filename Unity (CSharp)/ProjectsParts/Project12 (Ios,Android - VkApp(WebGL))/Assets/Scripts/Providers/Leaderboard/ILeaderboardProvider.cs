using System.Collections.Generic;
using Game.Scripts.Providers.Leaderboard.Base;

namespace Game.Scripts.Providers.Leaderboard
{
	public interface ILeaderboardProvider
	{
		public List<LeaderboardItem> LeaderboardList { get; }
	}
}