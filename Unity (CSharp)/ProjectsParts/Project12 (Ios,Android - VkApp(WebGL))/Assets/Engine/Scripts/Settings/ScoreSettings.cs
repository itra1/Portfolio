using System.Collections.Generic;
using Engine.Scripts.Settings.Common;
using UnityEngine;

namespace Engine.Scripts.Settings
{

	[System.Serializable]
	[CreateAssetMenu(fileName = "ScoreSettings", menuName = "Dypsloom/Rhythm Timeline/Score Setting", order = 1)]
	public class ScoreSettings : ScriptableObject, INoteAccuracySettings, IScorePerSong
	{
		[SerializeField] private float _scorePerSong = 500;
		[SerializeField] private string _accuracyLabelsResources;
		[SerializeField] private List<NoteAccuracy> _accuracyTable;

		public float ScorePerSong => _scorePerSong;
		public string AccuracyLabelsResources => _accuracyLabelsResources;
		public List<NoteAccuracy> AccuracyTable => _accuracyTable;
	}
}