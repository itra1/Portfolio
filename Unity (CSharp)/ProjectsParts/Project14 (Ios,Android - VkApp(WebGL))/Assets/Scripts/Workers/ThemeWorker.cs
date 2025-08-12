using Core.Engine.Components.Themes;
using Core.Engine.Components.Themes.Common;
using Scripts.Signals;
using Zenject;

namespace Scripts.Workers {
	public class ThemeWorker {
		private readonly SignalBus _signalBus;
		private readonly IThemeProvider _themeProvider;
		private readonly IThemeComponents _themeComponents;

		public ThemeWorker(SignalBus signalBus, IThemeProvider themeProvider, IThemeComponents themeComponents) {
			_themeProvider = themeProvider;
			_signalBus = signalBus;
			_themeComponents = themeComponents;

			_signalBus.Subscribe<LevelStartSignal>(OnLevelStartSignal);
		}

		private void OnLevelStartSignal(LevelStartSignal signal) {
			var activeitem = _themeProvider.GetList[(int)(signal.Level.Level % _themeProvider.GetList.Count)];
			_ = activeitem.Confirm(_themeComponents);
			_themeProvider.SetTheme(activeitem);
		}
	}
}
