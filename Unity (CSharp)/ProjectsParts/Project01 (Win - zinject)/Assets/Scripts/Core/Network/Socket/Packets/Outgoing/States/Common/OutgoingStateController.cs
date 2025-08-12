using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Zenject;

namespace Core.Network.Socket.Packets.Outgoing.States.Common
{
	/// <summary>
	/// Устаревшее название - "StateManager"
	/// </summary>
	public class OutgoingStateController : IOutgoingStateController, ILateTickable
	{
		private readonly IOutgoingState _outgoingState;
		
		private bool _isSendingAvailable;
		
		public bool IsSendingLocked { get; set; }
		
		public IOutgoingStateContext Context => _outgoingState;

		public OutgoingStateController(IOutgoingState outgoingState) =>
			_outgoingState = outgoingState;
		
		public void PrepareToSendIfAllowed()
		{
			_outgoingState.UpdateTimestamp();
			_isSendingAvailable = true;
		}
		
		public void ForceToSendIfAllowed()
		{
			_outgoingState.UpdateTimestamp();
			_outgoingState.AttemptToSendIfAllowed();
		}
		
		public void LateTick()
		{
			if (IsSendingLocked || !_isSendingAvailable) 
				return;
			
			_isSendingAvailable = false;
			
			_outgoingState.AttemptToSendIfAllowed();
		}
	}
}