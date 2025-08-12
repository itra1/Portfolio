using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Map.Data
{
    public class LocationType
    {
        [MaterialDataPropertyParse("id")]
        public int Id { get; set; }
        
        [MaterialDataPropertyParse("name")]
        public string Name { get; set; }
    }
}