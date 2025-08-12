using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Base;
using Core.Network.Socket.Packets.Incoming.Consts;
using Leguar.TotalJSON;

namespace Core.Network.Socket.Packets.Incoming.States
{
	public abstract class IncomingAction : IncomingPacket
	{
		[HideSocketEditor]
		public string Alias { get; private set; }
		
		protected JSON Content { get; private set; }
		protected JArray ContentArray { get; private set; }
		
		public void Parse(JSON data)
		{
			if (data.ContainsKey(IncomingPacketDataKey.Action))
				Alias = data.GetString(IncomingPacketDataKey.Action);
			
			if (data.ContainsKey(IncomingPacketDataKey.Content))
			{
				var content = data.Get(IncomingPacketDataKey.Content);
				
				switch (content)
				{
					case JSON:
						Content = data.GetJSON(IncomingPacketDataKey.Content);
						break;
					case JArray:
						ContentArray = data.GetJArray(IncomingPacketDataKey.Content);
						break;
				}
			}
			
			Parse();
		}

		public void ReplaceAlias(string newAlias) => Alias = newAlias;
	}
}