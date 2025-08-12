using System.Collections.Generic;
using Game.Base;
using Game.Game.Settings;
using Game.Providers.Battles.Base;
using StringDrop;
using Uuid;

namespace Game.Providers.Battles.Settings
{
	[System.Serializable]
	public struct DuelItemSettings
	{
		[UUID] public string Uuid;
		[StringDropList(typeof(BattleType))] public string Type;
		public string Title;
		public int PlayersCount;
		public int HitCount;
		public float WinRate;
		public int WinExp;
		public int Timer;
		public bool FirstGameWin;
		public bool UseBot;
		public BotSettings Bot;
		public CalculablePlayerResources Fee;
		public CalculablePlayerResources Reward;
		public List<DuelStageData> Stages;
	}
}
