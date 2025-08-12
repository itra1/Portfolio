using System.Linq;
using Game.Providers.Ui.Windows.Base;
using Game.Providers.Ui.Windows.Factorys;
using UnityEngine;

namespace Game.Providers.Ui.Windows {
	public class WindowProvider :IWindowProvider {
		private WindowFactory _screenFactory;
		private IWindowsParent _screenParent;
		private Presenter _openedWindow;

		public WindowProvider(WindowFactory windowFactory, IWindowsParent windowsParent) {
			_screenFactory = windowFactory;
			_screenParent = windowsParent;
		}

		public Presenter GetWindow(string name, bool isSplash = false) {
			Debug.Log("Try open window " + name);
			var targetScreen = _screenFactory.GetInstance(name, isSplash ? _screenParent.SplashParent : _screenParent.WindowsParent);

			if (targetScreen.Equals(_openedWindow))
				return _openedWindow;

			_openedWindow = _screenFactory.GetInstance(name, _screenParent.WindowsParent);

			var rt = _openedWindow.GetComponent<RectTransform>();
			rt.FullRect();
			return _openedWindow;
		}

		public Presenter GetActiveWindow() {
			return (from w in _screenParent.WindowsParent.GetComponentsInChildren<Presenter>()
							where w.gameObject.activeSelf
							select w).Last();
		}

		public void CloseAllWindows() {
			var windows = _screenParent.WindowsParent.GetComponentsInChildren<Presenter>();
			foreach (var w in windows)
				w.gameObject.SetActive(false);
		}
	}
}
