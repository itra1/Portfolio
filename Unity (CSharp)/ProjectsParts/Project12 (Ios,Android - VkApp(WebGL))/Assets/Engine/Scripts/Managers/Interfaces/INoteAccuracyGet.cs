using Engine.Scripts.Settings.Common;

namespace Engine.Engine.Scripts.Managers.Interfaces
{
	public interface INoteAccuracyGet
	{
		INoteAccuracy GetMissAccuracy();
		INoteAccuracy GetNoteAccuracy(float offsetPercentage);
		INoteAccuracy GetPerfectAccuracy();
	}
}
