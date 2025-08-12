using System.Collections.Generic;
using Engine.Scripts.Timelines;
using Game.Scripts.App;
using Game.Scripts.Providers.Songs.Saves;

namespace Game.Scripts.Providers.Songs.Helpers
{
	public interface ISongSaveHelper : IApplicationLoaderItem
	{
		int StarsSum { get; }

		List<string> OpenSongs { get; }

		SongScoreData GetEmptyScore();
		int[] GetNoteAccuracyIDCounts(string songUuid);
		SongScoreData GetScore(string songUuid);
		SongScoreData GetScore(RhythmTimelineAsset song);
	}
}