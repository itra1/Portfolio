using Core.Elements.Presentation.Data.Parsing;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.Presentation.Data
{
    [MaterialDataLoader("/presentation")]
    [MaterialDataParser(typeof(PresentationMaterialDataParser))]
    public class PresentationMaterialData : MaterialData
    {
        [MaterialDataPropertyParse("isActive"), MaterialDataPropertyUpdate]
        public bool IsActive { get; set; }
        
        [MaterialDataPropertyParse("isFavorite"), MaterialDataPropertyUpdate]
        public bool IsFavorite { get; set; }
        
        [MaterialDataPropertyParse("totalTiming"), MaterialDataPropertyUpdate]
        public float TotalTiming { get; set; }
        
        public override string UpdateMessageType => MessageType.PresentationMaterialDataUpdate;
    }
}