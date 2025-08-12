using System.Collections.Generic;
using Engine.Scripts.Base;
using Engine.Scripts.Timelines;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.Providers.Songs.Saves;
using UnityEngine;

namespace Game.Scripts.Providers.Songs.Helpers
{
	public class SongsHelper : ISongsHelper
	{
		private readonly ISongsProvider _songsProvider;
		private readonly ISongSaveHelper _songSaveHelper;
		private readonly IProfileProvider _profileProvider;

		public int StarsSum
			=> _songSaveHelper.StarsSum;

		public SongsHelper(
			ISongsProvider songsProvider,
			ISongSaveHelper songSaveHelper,
			IProfileProvider profileProvider
		)
		{
			_songsProvider = songsProvider;
			_songSaveHelper = songSaveHelper;
			_profileProvider = profileProvider;
		}

		/// <summary>
		/// Returns the list are available for songs for use
		/// </summary>
		/// <returns></returns>
		public List<RhythmTimelineAsset> GetReadySongs()
			=> _songsProvider.Songs.FindAll(x => x.ConditionStar <= StarsSum
			|| (x.IsSpecial && _songSaveHelper.OpenSongs.Contains(x.Uuid)));

		/// <summary>
		/// Returns a list of accessible
		/// </summary>
		/// <param name="dificultyTrack"></param>
		/// <returns></returns>
		public List<RhythmTimelineAsset> GetReadySongs(DifficultyTrack dificultyTrack)
			=> GetReadySongs().FindAll(x => (x.DificultyTrack & dificultyTrack) != 0);

		public List<RhythmTimelineAsset> GetTutorialSongs()
			=> GetReadySongs().FindAll(x => x.IsTutorial);

		/// <summary>
		/// Returns a single asset track
		/// </summary>
		/// <param name="songUuid">RhythmTimelineAsset.Uuid</param>
		/// <returns></returns>
		public RhythmTimelineAsset GetReadySong(string songUuid)
			=> GetReadySongs().Find(x => x.Uuid == songUuid);

		public SongScoreData GetEmptyScore()
		=> _songSaveHelper.GetEmptyScore();

		public SongScoreData GetScore(RhythmTimelineAsset song)
			=> _songSaveHelper.GetScore(song.Uuid);
		public SongScoreData GetScore(string songUuid)
		{
			var song = _songsProvider.GetSong(songUuid);
			return _songSaveHelper.GetScore(song);
		}

		public Texture2D GetCover(string songUuid)
		 => _songsProvider.GetCover(songUuid);
	}
}
