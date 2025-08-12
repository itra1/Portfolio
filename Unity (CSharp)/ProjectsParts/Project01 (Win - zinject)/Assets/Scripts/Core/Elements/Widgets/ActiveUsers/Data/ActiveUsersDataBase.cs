using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.ActiveUsers.Data
{
    public abstract class ActiveUsersDataBase : WidgetDataBase, ISelfDeserializable
    {
        [MaterialDataPropertyParse("day")]
        public long? DayCounts { get; set; }
        
        [MaterialDataPropertyParse("week")]
        public long? WeekCounts { get; set; }
        
        [MaterialDataPropertyParse("month")]
        public long? MonthCounts { get; set; }
        
        [MaterialDataPropertyParse("messagesCount")]
        public long? MessagesCount { get; set; }
        
        [MaterialDataPropertyParse("statistic")]
        public List<Record> Statistic { get; set; }
        
        [MaterialDataPropertyParse("date")]
        public string Date { get; set; }
        
        public System.DateTime DateTime { get; set; }
        
        public void Deserialize()
        {
            if (!string.IsNullOrEmpty(Date))
                DateTime = System.DateTime.Parse(Date);
            
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