using System;

namespace Core.Network.Socket.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SocketEventAttribute : SocketPacketAttribute
	{
		public override string PacketName { get; }
		public string Description { get; }
		public override string ReplaceName { get; }
		
		public SocketEventAttribute(string aliasName, string description = null, string replaceAliasName = null)
		{
			PacketName = aliasName;
			Description = description ?? string.Empty;
			ReplaceName = replaceAliasName;
		}
	}
}