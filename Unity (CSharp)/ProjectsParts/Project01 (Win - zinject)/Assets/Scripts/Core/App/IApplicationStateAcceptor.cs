using Core.Network.Socket.Packets.Incoming.States.Data;

namespace Core.App
{
    public interface IApplicationStateAcceptor
    {
        void Accept(IncomingStateData state, ulong? cursorId);
    }
}