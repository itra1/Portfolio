using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("video_reset")]
	public class VideoReset : IncomingAction
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
			
			//TODO: Why is the "VideoReset" message type duplicated?
			
			if (!string.IsNullOrEmpty(MasterMeetingPositionId))
				MessageDispatcher.SendMessage(this, MessageType.VideoReset, MasterMeetingPositionId, EnumMessageDelay.IMMEDIATE);
			
			if (!string.IsNullOrEmpty(WindowId))
				MessageDispatcher.SendMessage(this, MessageType.VideoReset, WindowId, EnumMessageDelay.IMMEDIATE);
			
			return true;
		}
	}
}