using System.Collections.Generic;
using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Consts;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Calendar.Data
{
	[WidgetDataTypeKey(WidgetDataTypeKey.CalendarV2)]
	public class CalendarV2Data : WidgetDataBase, ISelfDeserializable
	{
		[MaterialDataPropertyParse("records")] 
		public List<CalendarRecord> CalendarRecords { get; set; }

		public void Deserialize()
		{
			if (CalendarRecords != null)
			{
				foreach (var record in CalendarRecords)
				{
					var timeStart = record.TimeStart;
					var timeEnd = record.TimeEnd;
				
					if (!string.IsNullOrEmpty(timeStart))
						record.DateTimeStart = System.DateTime.Parse(timeStart);
					if (!string.IsNullOrEmpty(timeEnd))
						record.DateTimeEnd = System.DateTime.Parse(timeEnd);
				}
			}
		}
	}
}