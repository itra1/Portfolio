using System.Collections.Generic;
using Game.Game.Settings;
using Game.Providers.Battles.Base;
using Game.Providers.Battles.Interfaces;
using StringDrop;
using UnityEngine;
using Uuid;

namespace Game.Providers.Battles.Settings
{
	[CreateAssetMenu(fileName = "BattleDuelSettings", menuName = "Battle/Settings/DuelSettings")]
	public class BattleDuelSettings : BattleTypeSettings
	{
		[SerializeField] private double _maxTimeWait = 30;
		[SerializeField] private double _roundTime = 180;
		[SerializeField] private int _coinsWin = 250;
		[SerializeField] private int _coinsLoss = 50;
		[SerializeField] private List<DuelStageData> _stages;

		public double MaxTimeWait => _maxTimeWait;
		public double RoundTime => _roundTime;
		public List<DuelStageData> Stages => _stages;
		public int CoinsWin => _coinsWin;
		public int CoinsLoss => _coinsLoss;

		public struct DuelItemSettings : IDuelItemSettings
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
			public List<DuelStageData> Stages;
		}
	}
}
