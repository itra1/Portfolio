using Game.Common.Signals;

namespace Game.Providers.Ui.Windows.Signals {
	public class WindowVisibleChangeSignal :ISignal {
		public string ScreenType { get; set; }
		public bool IsVisible { get; set; }
	}
}
