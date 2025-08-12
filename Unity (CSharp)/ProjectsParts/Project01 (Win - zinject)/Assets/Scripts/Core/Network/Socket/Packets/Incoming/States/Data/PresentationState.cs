using Core.Materials.Attributes;

namespace Core.Network.Socket.Packets.Incoming.States.Data
{
    public class PresentationState
    {
        [MaterialDataPropertyParse("presentationId")]
        public ulong PresentationId { get; set; }
        
        [MaterialDataPropertyParse("episodeId")]
        public ulong EpisodeId { get; set; }
    }
}