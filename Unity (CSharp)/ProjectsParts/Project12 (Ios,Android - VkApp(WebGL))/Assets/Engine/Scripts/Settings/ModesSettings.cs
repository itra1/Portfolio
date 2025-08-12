using UnityEngine;

namespace Engine.Scripts.Settings
{
	[System.Serializable]
	public class ModesSettings
	{
		[SerializeField] private string _resourcesPath;

		public string ResourcesPath => _resourcesPath;
	}
}
