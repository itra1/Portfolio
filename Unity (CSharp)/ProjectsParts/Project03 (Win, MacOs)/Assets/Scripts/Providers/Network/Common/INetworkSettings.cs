
namespace Providers.Network.Common
{
	public interface INetworkSettings
	{
		public string ServerDev { get; }
		public string ServerProd { get; }
		public string Server { get; }
	}
}
