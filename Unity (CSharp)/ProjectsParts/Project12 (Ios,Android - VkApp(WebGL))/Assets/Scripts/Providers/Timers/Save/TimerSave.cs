using Game.Scripts.Providers.Saves.Data;

namespace Game.Scripts.Providers.Timers.Save
{
	internal class TimerSave : SaveProperty<TimerSaveData>
	{
		public override TimerSaveData DefaultValue => new();
	}
	public class TimerSaveData
	{

	}
}
