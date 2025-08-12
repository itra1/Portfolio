using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Weather.Data
{
    public class Record
    {
        [MaterialDataPropertyParse("city")]
        public string City { get; set; }
        
        [MaterialDataPropertyParse("camera")]
        public ulong CameraId { get; set; }
    }
}