using System;
using Core.Network.Socket.Enums;
using Core.Network.Socket.Packets.Incoming.Base;
using Core.Network.Socket.Packets.Incoming.Consts;

namespace Core.Network.Socket.Packets.Incoming.Error
{
	public class ErrorPacket : IncomingPacket
	{
		public SocketErrorCode Code { get; private set; }
		public string Message { get; private set; }

		public override bool Parse()
		{
			Code = Enum.Parse<SocketErrorCode>(DataJson.GetInt(IncomingPacketDataKey.Code).ToString());
			Message = DataJson.GetString(IncomingPacketDataKey.Message);
			return true;
		}
	}
}