using UnityEngine;

namespace Game.Providers.Ui.Windows.Base {
	public interface IWindowsParent {
		RectTransform WindowsParent { get; }
		RectTransform SplashParent { get; }
	}
}
