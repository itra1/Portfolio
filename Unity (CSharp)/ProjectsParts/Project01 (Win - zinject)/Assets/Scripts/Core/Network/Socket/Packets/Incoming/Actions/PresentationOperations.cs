using com.ootii.Messages;
using Core.Elements.Presentation.Data;
using Core.Messages;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("presentation_create")]
    [SocketAction("presentation_delete")]
    [SocketAction("presentation_update")]
    public class PresentationOperations : MaterialOperations<PresentationMaterialData>
    {
        public override bool Process()
        {
            if (!base.Process())
                return false;
			
            if (IsRemoving) 
                MessageDispatcher.SendMessage(this, MessageType.PresentationUnload, Material.Id, EnumMessageDelay.IMMEDIATE);
			
            return true;
        }
    }
}