using Core.Engine.Signals;

namespace Core.Engine.uGUI.Screens.Signals
{
	public class ScreenVisibleChangeSignal: ISignal
	{
		public string ScreenType { get; set; }
		public bool IsVisible { get; set; }
	}
}
