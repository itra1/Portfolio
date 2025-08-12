using Pipes.Data;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base
{
	public class SendMessage : IPackageData
	{
		public string OutgoingPacket { get; set; }
		public string IncomingPacket { get; set; }
		public bool Completed { get; set; }
	}
}
