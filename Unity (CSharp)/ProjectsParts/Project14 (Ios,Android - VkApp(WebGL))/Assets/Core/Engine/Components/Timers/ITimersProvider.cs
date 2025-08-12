namespace Core.Engine.Components.Timers
{
	public interface ITimersProvider
	{
		public ITimer Create(TimerType type);
	}
}
