using UnityEngine;

using Zenject;

namespace Settings.Themes
{
	public class ThemeProvider
	{
		private DiContainer _container;
		private ISettings _settings;
		private ITheme _theme;
		public ThemeProvider(DiContainer container, ISettings settings)
		{
			_container = container;
			_settings = settings;
			FindTheme();
		}

		private void FindTheme()
		{
			_theme = Resources.Load<Theme>(_settings.ThemesPath);
			_container.BindInterfacesAndSelfTo<Theme>().FromInstance(_theme);
			_container.Resolve<ITheme>();
		}

	}
}
