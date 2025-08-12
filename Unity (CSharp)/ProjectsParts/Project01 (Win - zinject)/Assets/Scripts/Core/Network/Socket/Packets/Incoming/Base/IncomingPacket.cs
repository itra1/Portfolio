using Core.Network.Socket.Packets.Incoming.Actions;
using Core.Network.Socket.Packets.Incoming.Consts;
using Leguar.TotalJSON;

namespace Core.Network.Socket.Packets.Incoming.Base
{
	/// <summary>
	/// Устаревшее название - "PacketIn"
	/// </summary>
	public abstract class IncomingPacket : IIncomingPacket, IRawString, IDataJsonSetter
	{
		public string PacketType { get; private set; }

		public string RawString { private get; set; }
		
		public JSON DataJson { protected get; set; }
		
		public virtual void Execute()
		{
			var dataJson = JArray.ParseString(RawString).GetJSON(1);
			
			if (dataJson.ContainsKey(IncomingPacketDataKey.Action))
				PacketType = dataJson.GetString(IncomingPacketDataKey.Action);
			
			DataJson = dataJson;
		}
		
		public virtual bool Parse() => false;
		public virtual bool Process() => true;
		
		public override string ToString() => RawString;
	}
}