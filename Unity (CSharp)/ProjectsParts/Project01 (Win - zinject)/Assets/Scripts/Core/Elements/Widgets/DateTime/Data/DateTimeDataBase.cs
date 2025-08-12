using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.DateTime.Data
{
    public class DateTimeDataBase : WidgetDataBase, ISelfDeserializable
    {
        [MaterialDataPropertyParse("city")]
        public string City { get; set; }

        [MaterialDataPropertyParse("oktmo")]
        public string Oktmo { get; set; }
		
        [MaterialDataPropertyParse("timezone")]
        public long Timezone { get; set; }

        [MaterialDataPropertyParse("holidays")]
        public List<Holiday> Holidays { get; set; }
        
        public void Deserialize()
        {
            if (Holidays != null)
            {
                foreach (var holiday in Holidays)
                {
                    if (!string.IsNullOrEmpty(holiday.Date))
                        holiday.DateTime = System.DateTime.Parse(holiday.Date);
                }
            }
        }
    }
}