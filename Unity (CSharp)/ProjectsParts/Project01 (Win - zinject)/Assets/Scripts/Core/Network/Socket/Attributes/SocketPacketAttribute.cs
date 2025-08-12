using System;

namespace Core.Network.Socket.Attributes
{
	public abstract class SocketPacketAttribute : Attribute
	{
		public abstract string PacketName { get; }
		public abstract string ReplaceName { get; }
	}
}