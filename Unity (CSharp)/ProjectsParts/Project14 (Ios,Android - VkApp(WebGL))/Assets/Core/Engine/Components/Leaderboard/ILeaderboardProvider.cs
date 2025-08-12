using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.Leaderboard
{
	public interface ILeaderboardProvider
	{
		public List<LeaderboarItem> GetAroundItems(float currentVal);
	}
}
