using Engine.Scripts.Base;
using StringDrop;
using UnityEngine;

namespace Engine.Scripts.Settings.Common
{
	[System.Serializable]
	public class NoteAccuracy : INoteAccuracy
	{
		[StringDropList(typeof(AccuracyNames))][SerializeField] private string _type;
		[SerializeField] private bool _active;
		[SerializeField] private bool _breakChain;
		[SerializeField] private float _percentageTheshold;
		[SerializeField] private float _score;

		public string Type => _type;
		public bool BreakChain => _breakChain;
		public float Score => _score;
		public float PercentageTheshold => _percentageTheshold;
		public bool Active => _active;
		public bool IsPerfect => _type == AccuracyNames.Perfect;
		public bool IsMiss => _type == AccuracyNames.Miss;

		public bool IsActualPercentageTheshold(float time) => time <= _percentageTheshold;
	}
}