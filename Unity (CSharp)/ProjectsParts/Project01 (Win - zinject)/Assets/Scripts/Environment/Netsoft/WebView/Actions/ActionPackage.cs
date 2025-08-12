using Pipes.Data;

namespace Environment.Netsoft.WebView.Actions
{
	public class ActionPackage : IPackageData
	{
		public string OutgoingPacket { get; set; }
		public string IncomingPacket { get; set; }
		public bool Completed { get; set; }
	}
}
