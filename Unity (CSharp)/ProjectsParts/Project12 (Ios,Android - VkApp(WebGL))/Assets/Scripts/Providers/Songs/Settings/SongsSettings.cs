using UnityEngine;

namespace Game.Scripts.Providers.Songs.Settings
{
	[System.Serializable]
	public class SongsSettings : ISongsSettings, ISongsAddressables, ISongsNoCover, ISongsResources
	{
		[SerializeField] private string _resourcesPath;
		[SerializeField] private Texture2D _noImageCover;
		[SerializeField] private string[] _addressableLibs;

		public string ResourcesPath => _resourcesPath;
		public Texture2D NoImageCover => _noImageCover;
		public string[] AddressableLibs => _addressableLibs;
	}
}
