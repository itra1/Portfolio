using UnityEngine;

namespace Game.Scripts.UI.Settings
{
	[System.Serializable]
	public class UiSettings
	{
		[SerializeField] private string _uiPresentersPrefabPath;
		public string UiPresentersPrefabPath => _uiPresentersPrefabPath;
	}
}
