using System.Collections.Generic;
using Core.Materials.Attributes;

namespace Core.Materials.Data
{
    public class CalendarDataSourceMaterialData : MaterialData
    {
        [MaterialDataPropertyParse("settings"), MaterialDataPropertyUpdate]
        public object Settings { get; set; }
        
        [MaterialDataPropertyParse("data"), MaterialDataPropertyUpdate]
        public List<CalendarData> CalendarData { get; set; }
        
        [MaterialDataPropertyParse("errors"), MaterialDataPropertyUpdate]
        public List<object> Errors { get; set; }
        
        [MaterialDataPropertyParse("enabled"), MaterialDataPropertyUpdate]
        public bool Enabled { get; set; }
        
        [MaterialDataPropertyParse("monitoring"), MaterialDataPropertyUpdate]
        public bool Monitoring { get; set; }
        
        [MaterialDataPropertyParse("updatedDate"), MaterialDataPropertyUpdate]
        public string UpdatedDate { get; set; }
    }

    public class CalendarData
    {
        [MaterialDataPropertyParse("room"), MaterialDataPropertyUpdate]
        public string Room { get; set; }

        [MaterialDataPropertyParse("title"), MaterialDataPropertyUpdate]
        public string Title { get; set; }

        [MaterialDataPropertyParse("timeEnd"), MaterialDataPropertyUpdate]
        public string TimeEnd { get; set; }

        [MaterialDataPropertyParse("authorAva"), MaterialDataPropertyUpdate]
        public string AuthorAva { get; set; }

        [MaterialDataPropertyParse("eventType"), MaterialDataPropertyUpdate]
        public int EventType { get; set; }

        [MaterialDataPropertyParse("timeStart"), MaterialDataPropertyUpdate]
        public string TimeStart { get; set; }

        [MaterialDataPropertyParse("authorName"), MaterialDataPropertyUpdate]
        public string AuthorName { get; set; }

        [MaterialDataPropertyParse("listenersCount"), MaterialDataPropertyUpdate]
        public int ListenersCount { get; set; }
    }
}