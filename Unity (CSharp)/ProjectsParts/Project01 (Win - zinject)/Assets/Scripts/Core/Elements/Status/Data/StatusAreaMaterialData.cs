using Core.Materials.Attributes;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.Status.Data
{
    [MaterialDataLoader("/status-area")]
    public class StatusAreaMaterialData : AreaMaterialData
    {
        [MaterialDataPropertyParse("statusId"), MaterialDataPropertyUpdate]
        public ulong StatusId { get; set; }
        
        public override string UpdateMessageType => MessageType.StatusAreaMaterialDataUpdate;
    }
}