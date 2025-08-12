using System.Collections.Generic;

namespace Core.Network.Socket.ActionTypes
{
	/// <summary>
	/// Устаревшее название - "Socket3Manager"
	/// </summary>
	public interface ISocketActionPackets
	{
		IReadOnlyDictionary<string, SocketActionTypeData> PacketTypes { get; }
	}
}