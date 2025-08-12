using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Consts;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets
{
    [PacketName(PacketName.PresentationSlideInfoResult)]
    public class PresentationSlideInfoResult : PacketBase
    {
        public int Slide { get; set; }
        public int TotalSlides { get; set; }
        
        public PresentationSlideInfoResult(int slide, int totalSlides)
        {
            Slide = slide;
            TotalSlides = totalSlides;
        }
    }
}