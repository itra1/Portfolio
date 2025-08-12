using Game.Providers.Ui.Windows.Base;
using UnityEngine;

namespace Game.Providers.Ui.Windows {
	[System.Serializable]
	public class WindowsSettings :IWindowsSettings {
		[SerializeField] private string _screensFolder = "Prefabs/UI/Screens";
		public string ScreensFolder => _screensFolder;
	}
}
