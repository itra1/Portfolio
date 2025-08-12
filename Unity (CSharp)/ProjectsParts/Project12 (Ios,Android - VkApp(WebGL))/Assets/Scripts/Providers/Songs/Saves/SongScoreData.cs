using System;
using System.Collections.Generic;
using Game.Scripts.Controllers.AccuracyLabels;
using Newtonsoft.Json;

namespace Game.Scripts.Providers.Songs.Saves
{
	[Serializable]
	public class SongScoreData
	{
		public string Uuid { get; set; }
		public int[] NoteAccuracyIDCounts { get; set; } = new int[5];
		public List<int> NoteAccuracyIDHistogram { get; set; } = new();
		public float Score { get; set; }
		public int MaxChain { get; set; }
		public int Stars { get; set; }
		[JsonIgnore] public IAccuracyController accuracyController { get; set; }

		public void Recalc()
		{
			var accuracyTable = accuracyController.OrderedAccuracyTable;
			NoteAccuracyIDCounts = new int[accuracyTable.Count];
			for (int i = 0; i < NoteAccuracyIDHistogram.Count; i++)
			{
				var noteAccuracyID = NoteAccuracyIDHistogram[i];

				if (noteAccuracyID < 0 || noteAccuracyID >= NoteAccuracyIDCounts.Length)
					continue;

				NoteAccuracyIDCounts[noteAccuracyID]++;
			}
		}
	}
}
