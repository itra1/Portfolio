using Core.Network.Socket.Packets.Incoming.States.Data;

namespace Core.Recovery
{
    public interface IPreviousSessionRecovery
    {
        void HandleStateData(IncomingStateData data);
        void SetDefaultDesktop();
    }
}