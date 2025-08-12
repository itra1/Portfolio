using System.Collections.Generic;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Actions.Data;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("statuscontent_create_many")]
    public class StatusContentAreaCreateMany : IncomingMaterialAction
    {
        public IReadOnlyList<StatusContentItemData> DataItems { get; private set; }
        
        public override bool Parse()
        {
            var contentArray = ContentArray;
            var dataItems = new StatusContentItemData[contentArray.Length];
            
            for (var i = 0; i < contentArray.Length; i++)
                dataItems[i] = new StatusContentItemData(contentArray.GetJSON(i));
            
            DataItems = dataItems;
            return true;
        }
        
        public override bool Process()
        {
            if (!base.Process())
                return false;
            
            MessageDispatcher.SendMessage(this, MessageType.StatusContentAreaCreateMany, this, EnumMessageDelay.NEXT_UPDATE);
            return true;
        }
    }
}