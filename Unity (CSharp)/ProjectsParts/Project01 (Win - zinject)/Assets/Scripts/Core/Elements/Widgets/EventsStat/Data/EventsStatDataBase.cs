using System;
using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;
using Debug = Core.Logging.Debug;

namespace Core.Elements.Widgets.EventsStat.Data
{
    public abstract class EventsStatDataBase : WidgetDataBase, ISelfDeserializable
    {
        [MaterialDataPropertyParse("timeStart")]
        public string DateStart { get; set; }
		
        [MaterialDataPropertyParse("timeEnd")]
        public string DateEnd { get; set; }
		
        [MaterialDataPropertyParse("dateFrom")]
        public string DateFrom { get; set; }
		
        [MaterialDataPropertyParse("all")]
        public int All { get; set; }
		
        [MaterialDataPropertyParse("allIncrement")]
        public int AllIncrement { get; set; }

        [MaterialDataPropertyParse("records")]
        public List<Record> Records { get; set; }
        
        public System.DateTime DateTimeStart { get; set; }
        public System.DateTime DateTimeEnd { get; set; }
        public System.DateTime DateTimeFrom { get; set; }
        
        public void Deserialize()
        {
	        if (Records != null)
	        {
		        foreach (ISelfDeserializable record in Records)
					record.Deserialize();
		        
		        try
		        {
			        if (!string.IsNullOrEmpty(DateStart))
				        DateTimeStart = System.DateTime.Parse(DateStart);
			        
			        if (!string.IsNullOrEmpty(DateEnd))
				        DateTimeEnd = System.DateTime.Parse(DateEnd);
			        
			        if (!string.IsNullOrEmpty(DateFrom))
				        DateTimeFrom = System.DateTime.Parse(DateFrom);
		        }
		        catch (Exception exception)
		        {
			        Debug.LogException(exception);
		        }
	        }
        }
    }
}