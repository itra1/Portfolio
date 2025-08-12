using Zenject;

namespace Core.Network.Socket.Packets.Outgoing.States.Common.Base
{
	/// <summary>
	/// Устаревшее название - "PacketOut"
	/// </summary>
	public abstract class OutgoingPacket
	{
		private readonly ISocketSender _sender;

		protected abstract string PacketType { get; }

		protected OutgoingPacket() =>
			_sender = ProjectContext.Instance.Container.Resolve<ISocketSender>();

		public void AttemptToSendIfAllowedSend()
		{
			_sender.Send(PacketType, this);
		}
	}
}