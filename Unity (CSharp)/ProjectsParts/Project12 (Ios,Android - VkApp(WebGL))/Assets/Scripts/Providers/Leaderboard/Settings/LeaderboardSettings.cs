using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Scripts.Providers.Avatars;
using Game.Scripts.Providers.Leaderboard.Base;
using Game.Scripts.Helpers;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Leaderboard.Settings
{
	[System.Serializable]
	public class LeaderboardSettings : ILeaderboardSettings
	{
		[SerializeField] private Vector2Int _diapazone;
		[SerializeField] private List<LeaderboardItem> _leaderboard = new();
		[SerializeField] private List<string> _nicknames = new();

		public Vector2Int Diapasoze => _diapazone;
		public List<LeaderboardItem> Leaderboard => _leaderboard;

		public void ReadsNicknames()
		{
			string filePath = Application.dataPath + @"\..\..\Resources\NickNames.txt";
			var list = ReadFileNicknames(filePath);

			_nicknames = new();

			for (int i = 0; i < list.Count; i++)
			{
				_nicknames.Add(list[i]);
			}
		}

		public void ReadDatas()
		{
			Leaderboard.Clear();
			IAvatarsProvider avatars = StaticContext.Container.TryResolve<IAvatarsProvider>();

			List<string> names = new(_nicknames);
			int count = 98;
			float maxValue = count;
			float itemSize = 1f / maxValue;

			float allSize = _diapazone.y - _diapazone.x;
			int index = -1;
			while (names.Count > 0 && _leaderboard.Count < count)
			{
				var itmName = names[Random.Range(0, names.Count)];
				_ = names.Remove(itmName);
				index++;

				_leaderboard.Add(new LeaderboardItem()
				{
					Index = (count - index),
					Nickname = itmName,
					Value = Mathf.Round(itemSize * index * allSize) + _diapazone.x,
					AvatarUuid = avatars.GetRandom().Uuid
				});
			}
			_leaderboard = _leaderboard.OrderBy(x => x.Index).ToList();
		}

		public static List<string> ReadFileNicknames(string filePath)
		{
			List<string> result = new();

			var lines = File.ReadAllLines(filePath);

			foreach (var line in lines)
			{
				var readLine = line.Trim();

				if (InputValidate.UserName(readLine) == 0 && !result.Contains(readLine))
					result.Add(readLine);
			}

			return result;
		}
	}
}
