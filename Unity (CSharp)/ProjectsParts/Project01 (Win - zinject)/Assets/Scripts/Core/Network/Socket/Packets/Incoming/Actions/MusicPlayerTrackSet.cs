using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("musicPlayer_track_set")]
	public class MusicPlayerTrackSet : IncomingAction
	{
		private ulong _trackId;
		
		public override bool Parse()
		{
			_trackId = Content.GetULong(IncomingPacketDataKey.TrackId);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.MusicPlayerTrackSet, _trackId, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}