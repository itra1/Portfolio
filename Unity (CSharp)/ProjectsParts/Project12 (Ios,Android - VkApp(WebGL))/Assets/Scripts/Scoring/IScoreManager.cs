using Engine.Scripts.Settings.Common;
using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Notes.Base;
using Game.Scripts.Providers.Songs.Saves;
using UnityEngine.Events;

namespace Game.Scripts.Scoring
{
	public interface IScoreManager
	{
		UnityEvent OnBreakChain { get; set; }
		UnityEvent<int> OnContinueChain { get; set; }
		UnityEvent<float> OnScoreChange { get; set; }
		UnityEvent<float> OnScoreVisualChange { get; set; }
		UnityEvent<int, float> OnStarChange { get; set; }
		UnityEvent<Note, INoteAccuracy> OnNoteScore { get; set; }
		UnityEvent<RhythmTimelineAsset, SongScoreData> OnNewHighScore { get; set; }
		UnityEvent OnNotesComplete { get; set; }
		int UseScore { get; }

		//UnityEvent<int> OnAddNewStarsEvent { get; set; }

		float GetChainPercentage();
		float GetScorePercentage();
	}
}