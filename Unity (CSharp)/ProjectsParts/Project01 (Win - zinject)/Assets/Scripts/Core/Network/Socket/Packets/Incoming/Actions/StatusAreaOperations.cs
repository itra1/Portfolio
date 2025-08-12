using Core.Elements.Status.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("statusarea_create")]
    [SocketAction("statusarea_delete")]
    [SocketAction("statusarea_update")]
    public class StatusAreaOperations : MaterialOperations<StatusAreaMaterialData>
    {
        
    }
}