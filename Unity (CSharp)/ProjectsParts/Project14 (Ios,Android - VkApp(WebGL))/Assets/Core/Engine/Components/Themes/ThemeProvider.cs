using Core.Engine.Components.Themes.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Core.Engine.Components.Themes {
	public class ThemeProvider : IThemeProvider {

		public UnityEvent<IThemeItem> OnThemeChanged = new();

		private readonly List<IThemeItem> _items = new();
		private readonly IThemeSettings _settings;
		private readonly SignalBus _signalBus;
		private readonly DiContainer _diContainer;

		public List<IThemeItem> GetList => _items;

		public IThemeItem ActiveTheme { get; private set; }

		public ThemeProvider(SignalBus signalBus, IThemeSettings settings, DiContainer diContainer) {
			_signalBus = signalBus;
			_diContainer = diContainer;
			_settings = settings;
			LoadItems();
		}

		private void LoadItems() {
			var items = Resources.LoadAll<Theme>(_settings.ThemesPath).ToList();

			foreach (var item in items) {
				if (item.TryGetComponent<IThemeItem>(out var itm)) {
					_items.Add(itm);
					_diContainer.Inject(itm);
				}
			}

			AppLog.Log($"Themes = {_items.Count}");
		}

		public void SetTheme(IThemeItem themeItem) {
			ActiveTheme = themeItem;
			OnThemeChanged?.Invoke(ActiveTheme);
		}
	}
}
