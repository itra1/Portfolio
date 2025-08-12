using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Consts;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets
{
    [PacketName(PacketName.CommonError)]
    public class CommonError : PacketBase
    {
        public string Message { get; set; }
        
        public CommonError(string message = null)
        {
            Message = !string.IsNullOrEmpty(message) ? message : "Unknown error";
        }
    }
}