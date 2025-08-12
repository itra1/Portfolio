
using System.Collections.Generic;

using UGui.Screens.Base;
using UGui.Screens.Factorys;
using UnityEngine;
using Screen = UGui.Screens.Base.Screen;

using Zenject;
using UGui.Screens.Elements;

namespace UGui.Screens
{
	public class ScreenProvider: IScreenProvider
	{
		private ScreenFactory _windowFactory;
		private IScreenParent _windowsParent;
		private Screen _openedWindow;
		private List<IScreen> _screens = new();
		public ScreenProvider(ScreenFactory windowFactory, IScreenParent windowsParent)
		{
			_windowFactory = windowFactory;
			_windowsParent = windowsParent;
		}

		public Screen OpenWindow(string name)
		{
			var targetScreen = _windowFactory.GetInstance(name, _windowsParent.ScreensParent);

			if (targetScreen.Equals(_openedWindow))
				return _openedWindow;

			if (_openedWindow != null)
				_openedWindow.Hide();

			_openedWindow = _windowFactory.GetInstance(name, _windowsParent.ScreensParent);

			RectTransform rt = _openedWindow.GetComponent<RectTransform>();
			rt.FullRect();
			_openedWindow.Show();
			return _openedWindow;
		}
	}
}
