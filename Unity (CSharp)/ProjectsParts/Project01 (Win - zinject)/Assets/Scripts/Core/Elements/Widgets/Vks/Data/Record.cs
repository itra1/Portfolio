using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Vks.Data
{
    public class Record
    {
        [MaterialDataPropertyParse("curParts")]
        public int Value { get; set; }
        
        [MaterialDataPropertyParse("date")]
        public string Date { get; set; }
        
        public System.DateTime DateTime { get; set; }
    }
}