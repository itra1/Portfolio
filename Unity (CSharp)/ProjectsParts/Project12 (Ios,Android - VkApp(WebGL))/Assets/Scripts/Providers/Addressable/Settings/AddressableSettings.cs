using UnityEngine;

namespace Game.Scripts.Providers.Addressable.Settings
{
	[System.Serializable]
	public class AddressableSettings : IAddressableSettings
	{
		[SerializeField] private string _server;
		[SerializeField] private string _catalogFile;

		public string Server => _server;
		public string CatalogFile => _catalogFile;

	}
}
