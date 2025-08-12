using Core.Materials.Attributes;

namespace Core.Network.Socket.Packets.Incoming.States.Data
{
    public class IncomingStateData
    {
        [MaterialDataPropertyParse("desktop")]
        public DesktopState Desktop { get; set; }

        [MaterialDataPropertyParse("presentation")]
        public PresentationState Presentation { get; set; }

        [MaterialDataPropertyParse("status")]
        public StatusState Status { get; set; }
        
        [MaterialDataPropertyParse("soundVolume")]
        public SoundVolumeState SoundVolume { get; set; }
        
        [MaterialDataPropertyParse("current_screen")]
        public string CurrentScreen { get; set; }
    }
}