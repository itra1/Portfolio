using System.Collections.Generic;
using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Consts;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.IncidentYear.Data
{
	[WidgetDataTypeKey(WidgetDataTypeKey.IncidentYear)]
	public class IncidentYearData : WidgetDataBase, ISelfDeserializable
	{
		[MaterialDataPropertyParse("records")] 
		public List<Record> Records { get; set; }
		
		public void Deserialize()
		{
			if (Records != null)
			{
				foreach (var record in Records)
				{
					var date = record.Date;
					
					if (!string.IsNullOrEmpty(date))
						record.DateTime = System.DateTime.Parse(date);
				}
			}
		}
	}
}