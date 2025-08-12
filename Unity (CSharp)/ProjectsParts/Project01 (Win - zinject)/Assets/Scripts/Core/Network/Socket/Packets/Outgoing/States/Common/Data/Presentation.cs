namespace Core.Network.Socket.Packets.Outgoing.States.Common.Data
{
    public class Presentation
    {
        public EpisodeInfo currentEpisodeInfo = new();
        public ulong episodeId;
        public ulong presentationId;
    }
}