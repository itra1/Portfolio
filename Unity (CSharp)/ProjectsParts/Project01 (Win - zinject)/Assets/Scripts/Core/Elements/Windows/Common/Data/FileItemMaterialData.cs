using Core.Materials.Attributes;

namespace Core.Elements.Windows.Common.Data
{
    public class FileItemMaterialData
    {
        [MaterialDataPropertyParse("url"), MaterialDataPropertyUpdate]
        public string Url { get; set; }
		
        [MaterialDataPropertyParse("path"), MaterialDataPropertyUpdate]
        public string Path { get; set; }
		
        [MaterialDataPropertyParse("width"), MaterialDataPropertyUpdate]
        public int Width { get; set; }
		
        [MaterialDataPropertyParse("height"), MaterialDataPropertyUpdate]
        public int Height { get; set; }
		
        [MaterialDataPropertyParse("order"), MaterialDataPropertyUpdate]
        public int Order { get; set; }
    }
}