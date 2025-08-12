namespace Core.Network.Socket
{
	public interface ISocketState
	{
		bool IsIncomingPacketExecutionLocked { get; set; }
		bool IsOutgoingPacketExecutionLocked { get; set; }
	}
}