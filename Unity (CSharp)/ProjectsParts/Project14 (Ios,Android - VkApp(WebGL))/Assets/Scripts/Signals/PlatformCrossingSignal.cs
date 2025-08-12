using Core.Engine.Signals;

namespace Scripts.Signals
{
	public class PlatformCrossingSignal :ISignal
	{
		public int PlatformsAll { get; set; }
		public int PlatformIndex { get; set; }
	}
}
