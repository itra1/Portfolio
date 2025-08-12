namespace Pipes.Data
{
	public interface IPackageData
	{
		string OutgoingPacket { get; set; }
		string IncomingPacket { get; set; }
		bool Completed { get; set; }
	}
}
