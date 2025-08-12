using System.Collections.Generic;
using Engine.Scripts.Settings.Common;

namespace Engine.Scripts.Settings
{
	public interface INoteAccuracySettings
	{
		string AccuracyLabelsResources { get; }
		List<NoteAccuracy> AccuracyTable { get; }
	}
}