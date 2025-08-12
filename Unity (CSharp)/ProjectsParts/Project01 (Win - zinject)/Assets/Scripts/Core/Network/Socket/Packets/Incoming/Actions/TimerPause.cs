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
	[SocketAction("timer_paused")]
	public class TimerPause : IncomingAction
	{
		public TimerType Type { get; private set; }
		public bool Paused { get; private set; }
		
		public override bool Parse()
		{
			if (Content.ContainsKey(IncomingPacketDataKey.Type))
				Type = (TimerType) Enum.Parse(typeof(TimerType), Content.GetString(IncomingPacketDataKey.Type).ToTitleCase());
			if (Content.ContainsKey(IncomingPacketDataKey.Paused))
				Paused = Content.GetBool(IncomingPacketDataKey.Paused);
			
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.TimerPause, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}