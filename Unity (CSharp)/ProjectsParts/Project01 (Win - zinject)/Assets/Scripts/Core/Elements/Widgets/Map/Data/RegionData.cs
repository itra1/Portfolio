using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Map.Data
{
    public class RegionData : IRegionData
    {
        [MaterialDataPropertyParse("oktmo")]
        public string Oktmo { get; set; }
        
        [MaterialDataPropertyParse("region")]
        public string Title { get; set; }
        
        [MaterialDataPropertyParse("value")]
        public float Value { get; set; }
    }
}