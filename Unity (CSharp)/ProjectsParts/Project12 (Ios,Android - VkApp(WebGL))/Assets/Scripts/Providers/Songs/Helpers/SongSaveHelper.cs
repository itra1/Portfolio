using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Engine.Scripts.Timelines;
using Game.Scripts.Controllers.AccuracyLabels;
using Game.Scripts.Providers.Saves;
using Game.Scripts.Providers.Songs.Saves;

namespace Game.Scripts.Providers.Songs.Helpers
{
	public class SongSaveHelper : ISongSaveHelper
	{
		private readonly ISaveProvider _saveProvider;
		private readonly IAccuracyController _accuracyController;
		private SongsSave _songSave;

		public int StarsSum => _songSave.Score.Sum(x => x.Stars);

		public List<string> OpenSongs => _songSave.OpenSongs;

		public SongSaveHelper(ISaveProvider saveProvider, IAccuracyController accuracyController)
		{
			_saveProvider = saveProvider;
			_accuracyController = accuracyController;
		}

		public async UniTask StartAppLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_songSave = _saveProvider.GetProperty<SongsSaveData>().Value;

			if (_songSave.OpenSongs.Count == 0)
				ReadFirst();

			await UniTask.Yield();
		}

		private void ReadFirst()
		{
			foreach (var item in _songSave.Score)
			{
				item.accuracyController = _accuracyController;
			}
		}

		public SongScoreData GetScore(RhythmTimelineAsset song) => GetScore(song.Uuid);

		public SongScoreData GetScore(string songUuid)
		{
			var songItem = _songSave.Score.Find(x => x.Uuid == songUuid);

			if (songItem == null)
			{
				songItem = new();
				_songSave.Score.Add(songItem);
			}
			songItem.accuracyController ??= _accuracyController;

			return songItem;
		}

		public SongScoreData GetEmptyScore()
		{
			SongScoreData result = new();
			result.accuracyController ??= _accuracyController;
			return result;
		}

		public int[] GetNoteAccuracyIDCounts(string songUuid)
		{
			var songScore = GetScore(songUuid);

			var accuracyTable = _accuracyController.OrderedAccuracyTable;
			var noteAccuracyIDCounts = new int[accuracyTable.Count];
			for (int i = 0; i < songScore.NoteAccuracyIDHistogram.Count; i++)
			{
				var noteAccuracyID = songScore.NoteAccuracyIDHistogram[i];

				if (noteAccuracyID < 0 || noteAccuracyID >= noteAccuracyIDCounts.Length)
					continue;

				noteAccuracyIDCounts[noteAccuracyID]++;
			}

			return noteAccuracyIDCounts;
		}

		private void Save()
		{
			_ = _saveProvider.Save();
		}
	}
}
