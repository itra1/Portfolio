using Core.Elements.Windows.Browser.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("browser_create")]
    [SocketAction("browser_delete")]
    [SocketAction("browser_update")]
    public class BrowserOperations : MaterialOperations<BrowserMaterialData>
    {
        
    }
}