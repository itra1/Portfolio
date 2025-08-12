using Core.Materials.Attributes;

namespace Core.Elements.Widgets.ActiveUsers.Data
{
    public class Record
    {
        [MaterialDataPropertyParse("value")]
        public int Value { get; set; }
        
        [MaterialDataPropertyParse("date")]
        public string Date { get; set; }
        
        public System.DateTime DateTime { get; set; }
    }
}