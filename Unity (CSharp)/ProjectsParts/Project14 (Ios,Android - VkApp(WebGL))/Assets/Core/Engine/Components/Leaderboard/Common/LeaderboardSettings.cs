using Core.Engine.Components.Avatars;
using Core.Engine.Helpers;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Core.Engine.Components.Leaderboard
{
	[System.Serializable]
	public struct LeaderboarItem
	{
		public int Index;
		public string Name;
		public float Value;
		public string AvatarName;
		public bool IsMe { get; set; }
	}

	[CreateAssetMenu(fileName = "LeaderboardSettings", menuName = "App/Leaderboard")]
	public class LeaderboardSettings : ScriptableObject
	{
		[SerializeField] private Vector2Int _diapazone;
		[SerializeField] private List<LeaderboarItem> _leaderboard;
		[SerializeField] private List<string> _nicknames;

		public Vector2Int Diapasoze => _diapazone;
		public List<LeaderboarItem> Leaderboard => _leaderboard;



		[ContextMenu("Read Files")]
		public void ReadsNicknames()
		{
			string filePath = Application.dataPath + @"\..\..\..\..\Resources\NickNames.txt";
			var list = FileTextReaderHelper.ReadFileNicknames(filePath);

			_nicknames = new();

			for (int i = 0; i < list.Count; i++){
				_nicknames.Add(list[i]);
			}
		}

		[ContextMenu("Read Datas")]
		public void ReadDatas(){

			Leaderboard.Clear();
			IAvatarsProvider avatars = StaticContext.Container.TryResolve<IAvatarsProvider>();

			List<string> names = new(_nicknames);
			int count = names.Count;
			float maxValue = names.Count;
			float itemSize = 1f / maxValue;

			float allSize = _diapazone.y - _diapazone.x;

			int index = -1;
			while(names.Count > 0){
				var itmName = names[Random.Range(0, names.Count)];
				names.Remove(itmName);
				index++;

				_leaderboard.Add(new LeaderboarItem() { 
				Index = (count - index),
				Name = itmName, 
				Value = Mathf.Round(itemSize * index * allSize) + _diapazone.x,
					AvatarName = avatars.GetRandom().Key
				});
			}
			_leaderboard = _leaderboard.OrderBy(x => x.Index).ToList();

		}

	}
}
