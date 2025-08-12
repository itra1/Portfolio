using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Widgets.Map.Data
{
    public class GeographyPoint
    {
        [MaterialDataPropertyParse("id")]
        public ulong Id { get; set; }

        [MaterialDataPropertyParse("name")]
        public string Name { get; set; }

        [MaterialDataPropertyParse("type")]
        public string Type { get; set; }

        [MaterialDataPropertyParse("oktmo")]
        public ulong Oktmo { get; set; }

        [MaterialDataPropertyParse("coordinates")]
        public Coordinate Coordinate { get; set; }
        
        [MaterialDataPropertyParse("googleData")]
        public JSON GoogleData { get; set; }
    }
}