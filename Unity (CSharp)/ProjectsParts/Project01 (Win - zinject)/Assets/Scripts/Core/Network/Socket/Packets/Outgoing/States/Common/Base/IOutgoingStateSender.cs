using Cysharp.Threading.Tasks;

namespace Core.Network.Socket.Packets.Outgoing.States.Common.Base
{
	/// <summary>
	/// Устаревшее название - "StateMessage"
	/// </summary>
	public interface IOutgoingStateSender
	{
		void UpdateTimestamp();
		UniTask AttemptToSendIfAllowed();
	}
}