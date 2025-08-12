using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base.Consts;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base.Data;
using Zenject;

namespace Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base
{
    /// <summary>
    /// Устаревшее название - "PacketOut"
    /// </summary>
    public abstract class OutgoingTimersTickPacket
    {
        private readonly ISocketSender _sender;
        private readonly OutgoingPacketData _data;
		
        protected abstract string PacketType { get; }
		
        protected OutgoingTimersTickPacket()
        {
            _sender = ProjectContext.Instance.Container.Resolve<ISocketSender>();
            _data = new OutgoingPacketData();
        }

        public virtual void Send()
        {
            _data.action = PacketType;
            _data.content = this;
            
            _sender.Send(OutgoingActionName.Default, _data);
        }
    }
}