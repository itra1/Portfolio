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
	[SocketAction("set_timer_alarm")]
	public class TimerSetAlarm : IncomingAction
	{
		public TimerType Type { get; private set; }
		public bool Alarm { get; private set; }

		public override bool Parse()
		{
			if (Content.ContainsKey(IncomingPacketDataKey.Type))
				Type = (TimerType) Enum.Parse(typeof(TimerType), Content.GetString(IncomingPacketDataKey.Type).ToTitleCase());
			if (Content.ContainsKey(IncomingPacketDataKey.Alarm))
				Alarm = Content.GetBool(IncomingPacketDataKey.Alarm);
			
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.TimerSetAlarm, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}