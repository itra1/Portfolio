using Core.Materials.Attributes;

namespace Core.Elements.Widgets.DateTime.Data
{
    public class Holiday
    {
        [MaterialDataPropertyParse("name")]
        public string Name { get; set; }

        [MaterialDataPropertyParse("date")]
        public string Date { get; set; }

        [MaterialDataPropertyParse("isDayOff")]
        public bool IsDayOff { get; set; }

        public System.DateTime DateTime { get; set; }
    }
}