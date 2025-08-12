
using UnityEngine;

namespace Settings
{
	[CreateAssetMenu(fileName = "Settings", menuName = "Settings/Settings")]
	public class Settings : ScriptableObject, ISettings
	{
		[SerializeField] public string _serverDev = "https://gc202201.com";
		[SerializeField] public string _serverProd = "https://garillacasino2.com";
		[SerializeField] public string _themePath = "Themes";
		[SerializeField] private string _screensFolder = "Prefabs/UI/Screens";

		public string ServerDev => _serverDev;
		public string ServerProd => _serverProd;
		public string Server => ServerDev;
		public string ThemesPath => _themePath;
		public string ScreensFolder => _screensFolder;
	}
}
