using Core.Materials.Attributes;

namespace Core.Elements.Widgets.IncidentYear.Data
{
    public class Record
    {
        [MaterialDataPropertyParse("work")]
        public int InWork { get; set; }
        
        [MaterialDataPropertyParse("toClose")]
        public int ToClose { get; set; }
        
        [MaterialDataPropertyParse("monitoring")]
        public int Monitoring { get; set; }
        
        [MaterialDataPropertyParse("archive")]
        public int Archive { get; set; }
        
        [MaterialDataPropertyParse("date")]
        public string Date { get; set; }
        
        public System.DateTime DateTime { get; set; }
    }
}