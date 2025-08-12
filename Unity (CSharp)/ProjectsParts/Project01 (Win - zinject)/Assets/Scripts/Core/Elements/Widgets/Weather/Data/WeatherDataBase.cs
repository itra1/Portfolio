using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Weather.Data
{
    public abstract class WeatherDataBase : WidgetDataBase
    {
        [MaterialDataPropertyParse("city")]
        public string City { get; set; }
		
        [MaterialDataPropertyParse("oktmo")]
        public string Oktmo { get; set; }
		
        [MaterialDataPropertyParse("camera")]
        public ulong CameraId { get; set; }
		
        [MaterialDataPropertyParse("extend")]
        public List<Record> Records { get; set; }
    }
}