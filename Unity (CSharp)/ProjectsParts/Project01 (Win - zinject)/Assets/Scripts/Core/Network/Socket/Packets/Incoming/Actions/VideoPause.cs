using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("video_pause")]
	public class VideoPause : IncomingAction
	{
		public string MasterMeetingPositionId { get; private set; }
		public string WindowId { get; private set; }

		public override bool Parse()
		{
			if (Content.ContainsKey(IncomingPacketDataKey.MasterMeetingPositionId))
				MasterMeetingPositionId = Content.GetString(IncomingPacketDataKey.MasterMeetingPositionId);
			if (Content.ContainsKey(IncomingPacketDataKey.WindowId))
				WindowId = Content.GetString(IncomingPacketDataKey.WindowId);
			
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			//TODO: Why is the "VideoPause" message type duplicated?
			
			if (!string.IsNullOrEmpty(MasterMeetingPositionId))
				MessageDispatcher.SendMessage(this, MessageType.VideoPause, MasterMeetingPositionId, EnumMessageDelay.IMMEDIATE);
			
			if (!string.IsNullOrEmpty(WindowId))
				MessageDispatcher.SendMessage(this, MessageType.VideoPause, WindowId, EnumMessageDelay.IMMEDIATE);
			
			return true;
		}
	}
}