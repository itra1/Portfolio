using System.Collections.Generic;
using System.Linq;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("window_action")]
	public class WindowAction : IncomingAction
	{
		private IReadOnlyDictionary<string, WindowData> _windows;

		public override bool Parse()
		{
			var json = DataJson.GetJSON(IncomingPacketDataKey.CurrentWindows);
			_windows = json.Keys.ToDictionary(key => key, key => new WindowData(json.GetJSON(key)));
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.WindowAction, _windows, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}