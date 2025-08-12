using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Weather.Data
{
    public class WeatherIcon
    {
        [MaterialDataPropertyParse("icon")]
        public string Icon { get; set; }
        
        public override string ToString() => Icon;
    }
}