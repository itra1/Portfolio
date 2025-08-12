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
    [SocketAction("set_timer_position")]
    public class TimerSetPosition : IncomingAction
    {
        public TimerType Type { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        
        public override bool Parse()
        {
            if (Content.ContainsKey(IncomingPacketDataKey.Type))
                Type = (TimerType) Enum.Parse(typeof(TimerType), Content.GetString(IncomingPacketDataKey.Type).ToTitleCase());
            if (Content.ContainsKey(IncomingPacketDataKey.X))
                X = Content.GetFloat(IncomingPacketDataKey.X);
            if (Content.ContainsKey(IncomingPacketDataKey.Y))
                Y = Content.GetFloat(IncomingPacketDataKey.Y);
			
            return true;
        }

        public override bool Process()
        {
            if (!base.Process())
                return false;
			
            MessageDispatcher.SendMessage(this, MessageType.TimerSetPosition, this, EnumMessageDelay.IMMEDIATE);
            return true;
        }
    }
}