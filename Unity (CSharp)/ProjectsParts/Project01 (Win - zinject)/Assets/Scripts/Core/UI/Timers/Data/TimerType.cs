using Core.Common.Attributes;

namespace Core.UI.Timers.Data
{
	public enum TimerType
	{
		[Name("hour")] Hour, 
		[Name("minute")] Minute,
		[Name("stopwatch")] Stopwatch
	}
}