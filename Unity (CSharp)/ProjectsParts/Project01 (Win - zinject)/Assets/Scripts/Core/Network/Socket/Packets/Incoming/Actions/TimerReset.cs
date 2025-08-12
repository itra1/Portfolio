using System;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;
using Core.UI.Timers.Data;
using Core.Utils;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("reset_timer")]
	public class TimerReset : IncomingAction
	{
		public TimerType Type { get; private set; }
		
		public override bool Parse()
		{
			if (Content.ContainsKey(IncomingPacketDataKey.Type))
				Type = (TimerType) Enum.Parse(typeof(TimerType), Content.GetString(IncomingPacketDataKey.Type).ToTitleCase());
			
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.TimerReset, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}