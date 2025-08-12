using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Vks.Data
{
    public class VksDataBase : WidgetDataBase, ISelfDeserializable
    {
        [MaterialDataPropertyParse("currentValue")]
        public long CurrentValue { get; set; }
        
        [MaterialDataPropertyParse("maxValue")]
        public long MaxValue { get; set; }
        
        [MaterialDataPropertyParse("records")]
        public List<Record> Statistic { get; set; }
        
        public void Deserialize()
        {
            if (MaxValue < 270)
                MaxValue = 270;
            
            if (Statistic != null)
            {
                foreach (var record in Statistic)
                {
                    var date = record.Date;
                    
                    if (!string.IsNullOrEmpty(date))
                        record.DateTime = System.DateTime.Parse(date);
                }
            }
        }
    }
}