using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Map.Data
{
    public class MapLegend
    {
        [MaterialDataPropertyParse("path")] 
        public string Path { get; set; }
        
        [MaterialDataPropertyParse("mimetype")] 
        public string MimeType { get; set; }
        
        [MaterialDataPropertyParse("url")] 
        public string Url { get; set; }
    }
}