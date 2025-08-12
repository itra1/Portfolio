using System.Collections.Generic;
using Core.Engine.Signals;
using UnityEngine;
using Zenject;

namespace Core.Engine.uGUI.Screens
{
	public class ScreensProvider :IScreensProvider
	{
		private ScreenFactory _windowFactory;
		private IScreensParent _windowsParent;
		private Screen _openedWindow;
		private List<IScreen> _panels = new();
		private SignalBus _signalBus;

		public ScreensProvider(SignalBus signalBus, ScreenFactory windowFactory, IScreensParent windowsParent)
		{
			_signalBus = signalBus;
			_windowFactory = windowFactory;
			_windowsParent = windowsParent;

			_signalBus.Subscribe<UGUIButtonClickSignal>(OpenWindowClick);
		}

		private void OpenWindowClick(UGUIButtonClickSignal signal)
		{

			if (signal.Name == ButtonTypes.SettingsOpen)
			{
				OpenWindow(ScreenTypes.Settings);
				return;
			}
			if (signal.Name == ButtonTypes.DailyBonusOpen)
			{
				OpenWindow(ScreenTypes.DailyBonus);
				return;
			}
			if (signal.Name == ButtonTypes.FirstMenuOpen)
			{
				OpenWindow(ScreenTypes.FirstPage);
				return;
			}
			if (signal.Name == ButtonTypes.Shop)
			{
				OpenWindow(ScreenTypes.Shop);
				return;
			}
			if (signal.Name == ButtonTypes.Skins)
			{
				OpenWindow(ScreenTypes.Skins);
				return;
			}

		}

		public Screen OpenWindow(string name)
		{
			Debug.Log("Try open window " + name);
			var targetScreen = _windowFactory.GetInstance(name, _windowsParent.ScreenParent);
			targetScreen.SetScreenType(name);

			if (targetScreen.Equals(_openedWindow))
				return _openedWindow;

			if (_openedWindow != null)
				_openedWindow.Hide();

			_openedWindow = _windowFactory.GetInstance(name, _windowsParent.ScreenParent);

			var rt = _openedWindow.GetComponent<RectTransform>();
			rt.FullRect();
			_openedWindow.Show();
			return _openedWindow;
		}
	}
}