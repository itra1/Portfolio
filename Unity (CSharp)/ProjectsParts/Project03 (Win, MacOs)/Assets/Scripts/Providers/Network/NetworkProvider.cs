using Cysharp.Threading.Tasks;

using Providers.Network.Common;


namespace Providers.Network
{
	public partial class NetworkProvider : INetworkProvider, INetworkApi
	{
		private INetworkSettings _settings;
		private string _token;
		private string Token => _token;
		public string Server => _settings.Server;

		public NetworkProvider(INetworkSettings settings)
		{
			_settings = settings;
		}
	}
}
