using Game.Scripts.Providers.DailyMissions.Base;
using StringDrop;
using UnityEngine;

namespace Game.Scripts.Providers.DailyMissions.Settings
{
	[System.Serializable]
	public class DailyMissionIcone
	{
		[SerializeField][StringDropList(typeof(DailyMissionIconeType))] private string _type;
		[SerializeField] private Sprite _icone;

		public string Type => _type;
		public Sprite Icone => _icone;
	}

}
