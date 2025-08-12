using Leguar.TotalJSON;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	public interface IDataJsonSetter
	{
		public JSON DataJson { set; }
	}
}