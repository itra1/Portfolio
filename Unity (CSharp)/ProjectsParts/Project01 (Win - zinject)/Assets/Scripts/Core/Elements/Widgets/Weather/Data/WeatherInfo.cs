using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Weather.Data
{
    public class WeatherInfo
    {
        [MaterialDataPropertyParse("name")]
        public string Name { get; set; }
        
        [MaterialDataPropertyParse("main/temp")]
        public float Temp { get; set; }
        
        [MaterialDataPropertyParse("weather")]
        public WeatherIcon[] Icon { get; set; }
        
        [MaterialDataPropertyParse("timezone")]
        public double Timezone { get; set; }
        
        [MaterialDataPropertyParse("wind/speed")]
        public float Wind { get; set; }
        
        [MaterialDataPropertyParse("main/humidity")]
        public float Humidity { get; set; }
        
        public override string ToString() =>
            $"{{Name: {Name}, Temp: {Temp}, Icon: [{string.Join<WeatherIcon>(", ", Icon)}], Timezone: {Timezone}, Wind: {Wind}, Humidity: {Humidity}}}";
    }
}