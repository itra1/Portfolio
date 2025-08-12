using System.Collections.Generic;
using Engine.Scripts.Settings.Common;

namespace Game.Scripts.Controllers.AccuracyLabels
{
	public interface IAccuracyController
	{
		float MaxNoteScore { get; }
		IReadOnlyList<INoteAccuracy> OrderedAccuracyTable { get; }

		int GetID(INoteAccuracy noteAccuracy);
		INoteAccuracy GetMissAccuracy();
		INoteAccuracy GetNoteAccuracy(float offsetPercentage);
		void SpawnLabel(string accuraryType);
	}
}