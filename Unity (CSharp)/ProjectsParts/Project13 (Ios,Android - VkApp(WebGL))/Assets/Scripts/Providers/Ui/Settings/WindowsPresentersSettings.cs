using UnityEngine;

namespace Game.Providers.Ui.Settings
{
	[System.Serializable]
	public class WindowsPresentersSettings : IWindowsPresentersSettings
	{
		[SerializeField] private string _uiPresentersPrefabPath;

		public string UiPresentersPrefabPath => _uiPresentersPrefabPath;
	}
}