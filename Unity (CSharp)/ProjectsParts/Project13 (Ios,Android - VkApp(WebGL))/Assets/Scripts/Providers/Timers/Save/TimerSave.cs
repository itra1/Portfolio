using Game.Providers.Saves.Data;

namespace Game.Providers.Timers.Save
{
	internal class TimerSave : SaveProperty<TimerSaveData>
	{
		public override TimerSaveData DefaultValue => new();
	}
	public class TimerSaveData
	{

	}
}
