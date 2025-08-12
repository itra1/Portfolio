using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Engine.Engine.Scripts.Managers.Interfaces;
using Engine.Scripts.Settings;
using Engine.Scripts.Settings.Common;
using Game.Scripts.Controllers.AccuracyLabels.Factorys;

namespace Game.Scripts.Controllers.AccuracyLabels
{
	public class AccuracyController : IAccuracyController, INoteAccuracyGet
	{
		private readonly IAccuracyLabelsFactory _accuracyLabelsFactory;
		private readonly INoteAccuracySettings _noteAccuracySettings;

		private Dictionary<string, INoteAccuracy> _accuracyDictionary = new();
		private bool _isVisibleLabel = false;

		private List<INoteAccuracy> AccuracyTable { get; set; }
		public IReadOnlyDictionary<string, INoteAccuracy> AccuracyDictionary => _accuracyDictionary;
		public IReadOnlyList<INoteAccuracy> OrderedAccuracyTable => _noteAccuracySettings.AccuracyTable;
		public float MaxNoteScore { get; private set; }

		public AccuracyController(
			IAccuracyLabelsFactory accuracyLabelsFactory,
			INoteAccuracySettings noteAccuracySettings
		)
		{
			_accuracyLabelsFactory = accuracyLabelsFactory;
			_noteAccuracySettings = noteAccuracySettings;

			AccuracyTable = _noteAccuracySettings.AccuracyTable.OrderBy(x => x.PercentageTheshold).ToList<INoteAccuracy>();

			_accuracyDictionary.Clear();
			for (int i = 0; i < AccuracyTable.Count; i++)
				_accuracyDictionary.Add(AccuracyTable[i].Type, AccuracyTable[i]);
			MaxNoteScore = GetNoteAccuracy(0).Score;
		}

		public INoteAccuracy GetNoteAccuracy(float offsetPercentage)
		{
			INoteAccuracy last = null;

			for (int i = 0; i < AccuracyTable.Count; i++)
			{
				if (AccuracyTable[i].IsMiss)
					continue;

				if (AccuracyTable[i].IsActualPercentageTheshold(offsetPercentage))
					return AccuracyTable[i];

				last = AccuracyTable[i];
			}

			return last;
		}

		public INoteAccuracy GetMissAccuracy()
			=> AccuracyTable.Find(x => x.IsMiss);
		public INoteAccuracy GetPerfectAccuracy()
			=> AccuracyTable.Find(x => x.IsPerfect);

		public int GetID(INoteAccuracy noteAccuracy)
			=> AccuracyTable.IndexOf(noteAccuracy);

		public void SpawnLabel(string accuraryType)
		{
			if (_isVisibleLabel)
				return;

			_ = LabelVisible(accuraryType);
		}

		private async UniTask LabelVisible(string accuraryType)
		{
			_isVisibleLabel = true;
			var instance = _accuracyLabelsFactory.GetInstance(accuraryType);

			if (instance == null)
			{
				_isVisibleLabel = false;
				return;
			}

			await instance.VisibleAnimation();
			_isVisibleLabel = false;
		}
	}
}
