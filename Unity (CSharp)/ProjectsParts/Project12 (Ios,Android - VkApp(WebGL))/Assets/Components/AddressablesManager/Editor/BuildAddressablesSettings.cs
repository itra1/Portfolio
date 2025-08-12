using UnityEngine;

namespace AddressablesManager.Editor
{
	[CreateAssetMenu(fileName = "BuildAddressablesSettings", menuName = "Settings/Editor/BuildAddressablesSettings")]
	public class BuildAddressablesSettings : ScriptableObject
	{
		[SerializeField] private string _platform = "WebGL";
		[SerializeField] private Ftp _ftp;

		public Ftp Ftp => _ftp;
		public string Platform => _platform;
	}

	[System.Serializable]
	public class Ftp
	{
		[SerializeField] private string _host;
		[SerializeField] private string _userName;
		[SerializeField] private string _password;
		[SerializeField] private string _addressablePath;

		public string Host => _host;
		public string UserName => _userName;
		public string Password => _password;
		public string AddressablePath => _addressablePath;
	}
}
