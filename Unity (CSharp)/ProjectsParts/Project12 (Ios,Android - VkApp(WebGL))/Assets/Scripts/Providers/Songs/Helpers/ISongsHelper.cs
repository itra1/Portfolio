using System.Collections.Generic;
using Engine.Scripts.Base;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Songs.Saves;
using UnityEngine;

namespace Game.Scripts.Providers.Songs.Helpers
{
	public interface ISongsHelper
	{
		int StarsSum { get; }

		List<RhythmTimelineAsset> GetReadySongs();
		RhythmTimelineAsset GetReadySong(string songUuid);
		List<RhythmTimelineAsset> GetReadySongs(DifficultyTrack dificultyTrack);
		SongScoreData GetScore(string songUuid);
		List<RhythmTimelineAsset> GetTutorialSongs();
		SongScoreData GetEmptyScore();
		Texture2D GetCover(string songUuid);
	}
}