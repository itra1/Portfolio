using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("visible_sys_windows")]
	public class PresentationSystemWindowsVisible : IncomingAction
	{
		public bool Visible { get; private set; }
		
		public override bool Parse()
		{
			if (Content.ContainsKey(IncomingPacketDataKey.Visible))
				Visible = Content.GetBool(IncomingPacketDataKey.Visible);
			
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.PresentationSystemWindowsVisible, Visible, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}