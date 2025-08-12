namespace Core.Network.Socket.IgnoredPackets
{
	public interface IIgnoredIncomingPackets
	{
		bool IsPacketIgnored(string action);
	}
}