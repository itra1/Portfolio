using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States.Data;
using Leguar.TotalJSON;

namespace Core.Network.Socket.Packets.Incoming.Actions.Data
{
    public class PresentationData
    {
        public ulong PresentationId { get; }
        public ulong EpisodeId { get; }

        public PresentationData(JSON data)
        {
            PresentationId = data.GetULong(IncomingPacketDataKey.PresentationId);
            EpisodeId = data.GetULong(IncomingPacketDataKey.Id);
        }
        
        public PresentationData(PresentationState state)
        {
            PresentationId = state.PresentationId;
            EpisodeId = state.EpisodeId;
        }
    }
}