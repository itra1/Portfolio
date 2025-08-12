namespace Core.Network.Socket.Packets.Outgoing.States.Common.Base
{
	public interface IOutgoingStateController
	{
		bool IsSendingLocked { get; set; }
		
		IOutgoingStateContext Context { get; }
		
		void PrepareToSendIfAllowed();
		void ForceToSendIfAllowed();
	}
}