using System;

namespace Core.Network.Socket.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SocketActionAttribute : SocketPacketAttribute
	{
		public override string PacketName { get; }
		public override string ReplaceName { get; }
		
		public SocketActionAttribute(string aliasName, string replaceAliasName = null)
		{
			PacketName = aliasName;
			ReplaceName = replaceAliasName;
		}
	}
}