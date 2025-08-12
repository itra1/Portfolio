using System.Collections.Generic;
using System.Linq;
using Engine.Engine.Scripts.Managers.Interfaces;
using Engine.Scripts.Settings;
using Engine.Scripts.Settings.Common;
using Zenject;

namespace Engine.Editor.Scripts.Controllers
{
	public class AccuracyControllerEditor : INoteAccuracyGet
	{
		private INoteAccuracySettings _noteAccuracySettings;
		private List<INoteAccuracy> AccuracyTable { get; set; }

		[Inject]
		private void Constructor(INoteAccuracySettings noteAccuracySettings)
		{
			_noteAccuracySettings = noteAccuracySettings;
			AccuracyTable = _noteAccuracySettings.AccuracyTable.OrderBy(x => x.PercentageTheshold).ToList<INoteAccuracy>();
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
	}
}
