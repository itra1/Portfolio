using System.Collections.Generic;
using Game.Game.Elements.Barriers;
using Game.Game.Elements.Bonuses;
using Game.Providers.Battles.Interfaces;
using StringDrop;
using UnityEngine;

namespace Game.Game.Settings
{
	[System.Serializable]
	public struct DuelStageData : IDuelStageData
	{
		[SerializeField] private int _boardHits;
		[SerializeField] private int _spikes;
		[SerializeField] private bool _moveBarrier;
		[SerializeField] private bool _loseBarrier;
		[SerializeField] private bool _screwBarrier;
		[SerializeField] private RotationBoardOptionsStruct _boardRotation;

		public readonly int Spikes => _spikes;
		public readonly bool MoveBarrier => _moveBarrier;
		public readonly bool LoseBarrier => _loseBarrier;
		public readonly bool ScrewBarrier => _screwBarrier;
		public readonly int BoardHits => _boardHits;
		public readonly RotationBoardOptionsStruct BoardRotation => _boardRotation;
	}

	[System.Serializable]
	public struct RotationBoardOptionsStruct
	{
		public int[] Signs;
		public RangeFloat Speeds;
		public RangeFloat SpeedChange;
		public RangeFloat TimeRotation;
	}

	[System.Serializable]
	public struct BoardFormationItem
	{
		public string Type;
		public string SybType;
		public Vector3 LocalScale;
		public Vector3 LocalRotation;
		public Vector3 LocalPosition;
	}

	[System.Serializable]
	public struct BoardBarrier
	{
		[StringDropList(typeof(BarrierNames))] public List<string> Types;
		public RangeInt Counts;
		public RangeFloat Distances;
	}

	[System.Serializable]
	public struct BonusStruct
	{
		[StringDropList(typeof(BonusNames))] public List<string> Types;
		public RangeInt Counts;
		public RangeFloat Distances;
	}
}
