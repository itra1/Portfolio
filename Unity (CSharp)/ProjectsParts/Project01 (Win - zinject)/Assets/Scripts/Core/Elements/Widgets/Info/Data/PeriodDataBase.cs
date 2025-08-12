using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Info.Data
{
    public abstract class PeriodDataBase : WidgetDataBase, ISelfDeserializable
    {
        [MaterialDataPropertyParse("dateStart")] 
        public string DateStart { get; set; }
        
        [MaterialDataPropertyParse("dateEnd")]
        public string DateEnd { get; set; }
        
        public System.DateTime DateTimeStart { get; set; }
        public System.DateTime DateTimeEnd { get; set; }
        
        public void Deserialize()
        {
            if (!string.IsNullOrEmpty(DateStart))
                DateTimeStart = System.DateTime.Parse(DateStart);
            if (!string.IsNullOrEmpty(DateEnd))
                DateTimeEnd = System.DateTime.Parse(DateEnd);
        }
    }
}