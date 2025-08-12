namespace Core.Network.Socket.Packets.Incoming.Base
{
	/// <summary>
	/// Устаревшее название - "PacketIn"
	/// </summary>
	public interface IIncomingPacket
	{
		string PacketType { get; }

		void Execute();
		bool Parse();
		bool Process();
	}
}