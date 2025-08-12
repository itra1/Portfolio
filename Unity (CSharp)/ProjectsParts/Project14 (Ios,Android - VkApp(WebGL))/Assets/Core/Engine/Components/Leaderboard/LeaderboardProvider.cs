using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Core.Engine.Components.User;
using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.Leaderboard
{
	public class LeaderboardProvider : ILeaderboardProvider
	{
		private readonly ILeaderboardSettings _settings;
		private readonly IUserProvider _userProvider;
		private readonly LeaderboardSettings _usersList;
		private readonly List<LeaderboarItem> _users;
		private GameLevelSG _gameLevel;

		public LeaderboardProvider(ILeaderboardSettings settings
		, IUserProvider userProvider
		, SaveGameProvider saveGameProvider
		)
		{
			_settings = settings;
			_userProvider = userProvider;
			_usersList = Resources.Load<LeaderboardSettings>(_settings.LeaderboardSettings);
			_users = _usersList.Leaderboard;
			_gameLevel = (GameLevelSG)saveGameProvider.GetProperty<GameLevelSG>();
		}

		public List<LeaderboarItem> GetAroundItems(float currentVal){

			List<LeaderboarItem> result = new();


			var down = (from u in _users
								where u.Value <= currentVal
								orderby u.Value descending
								select u).ToList();

			for (int i = 0; i < down.Count && i < 5; i++)
			{
				result.Add(down[0]);
			}

			var inx = down.Count <= 0 ? _users[_users.Count-1].Index + 1 : down[0].Index - 1;

			result.Add(new LeaderboarItem() { Index = inx, Name = _userProvider.UserName, Value = _gameLevel.Value, IsMe = true });

			var up = (from u in _users
								where u.Value > currentVal
								orderby u.Value
								select u).ToList();

			int maxItems = 18 - result.Count;

			for (int i = 0; i < up.Count && i < maxItems; i++) {

				var itm = up[i];
				itm.Value += 1;
				result.Add(itm);
			}

			return result.OrderBy(x=>x.Index).ToList();

		}


	}
}
