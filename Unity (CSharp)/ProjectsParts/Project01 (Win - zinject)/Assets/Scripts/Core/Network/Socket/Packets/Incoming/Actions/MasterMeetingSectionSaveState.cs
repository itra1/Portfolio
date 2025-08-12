using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("mastermeeting_save_section_state")]
	public class MasterMeetingSectionSaveState : IncomingAction
	{
		public ulong MasterMeetingId { get; private set; }
		public ulong EpisodeId { get; private set; }
		public ulong SectionId { get; private set; }

		public override bool Parse()
		{
			if (Content.ContainsKey(IncomingPacketDataKey.MasterMeetingId))
				MasterMeetingId = Content.GetULong(IncomingPacketDataKey.MasterMeetingId);
			if (Content.ContainsKey(IncomingPacketDataKey.EpisodeId))
				EpisodeId = Content.GetULong(IncomingPacketDataKey.EpisodeId);
			if (Content.ContainsKey(IncomingPacketDataKey.SectionId))
				SectionId = Content.GetULong(IncomingPacketDataKey.SectionId);
			
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.MasterMeetingSectionStateSave, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}