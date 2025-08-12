using Engine.Scripts.Base;
using Game.Scripts.Providers.DailyMissions.Base;
using StringDrop;
using UnityEngine;
using Uuid;

namespace Game.Scripts.Providers.DailyMissions.Settings
{
	[System.Serializable]
	public class MissionItem
	{
		[StringDropList(typeof(DailyMissionType))]
		[SerializeField] private string _type;
		[SerializeField][UUID] private string _uuid;
		[SerializeField] private string _title;
		[SerializeField] private string _description;
		[SerializeField] private int _count = 1;
		[StringDropList(typeof(ColorTypes))]
		[SerializeField] private string _color;
		[StringDropList(typeof(DailyMissionIconeType))]
		[SerializeField] private string _icone;
		[SerializeField] private MissionNotificationType _notificationType;

		public string Type => _type;
		public string Uuid => _uuid;
		public int Count => _count;
		public string Color => _color;
		public string Icone => _icone;
		public string Title => _title;
		public string Description => _description;
		public MissionNotificationType NotificationType => _notificationType;
	}
}
