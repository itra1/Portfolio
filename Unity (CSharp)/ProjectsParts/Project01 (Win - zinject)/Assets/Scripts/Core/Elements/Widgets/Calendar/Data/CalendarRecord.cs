using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Calendar.Data
{
    public class CalendarRecord
    {
        [MaterialDataPropertyParse("title")]
        public string Title { get; set; }
        
        [MaterialDataPropertyParse("authorName")]
        public string Author { get; set; }
        
        [MaterialDataPropertyParse("authorAva")]
        public string AuthorAva { get; set; }
        
        [MaterialDataPropertyParse("listenersCount")]
        public int ListenersCount { get; set; }
        
        [MaterialDataPropertyParse("timeStart")]
        public string TimeStart { get; set; }
        
        [MaterialDataPropertyParse("timeEnd")]
        public string TimeEnd { get; set; }
        
        [MaterialDataPropertyParse("room")]
        public string Room { get; set; }
        
        [MaterialDataPropertyParse("eventType")]
        public int EventType { get; set; }
        
        public System.DateTime DateTimeStart { get; set; }
        public System.DateTime DateTimeEnd { get; set; }
    }
}