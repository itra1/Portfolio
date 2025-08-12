using System;
using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;
using Debug = Core.Logging.Debug;

namespace Core.Elements.Widgets.KcTable.Data
{
    public abstract class KcTableDataBase : WidgetDataBase, ISelfDeserializable
    {
        [MaterialDataPropertyParse("records")]
        public List<Record> Records { get; set; }
        
        public void Deserialize()
        {
            if (Records != null)
            {
                try
                {
                    foreach (var record in Records)
                    {
                        record.StatusOrder = GetStatusOrder(record.Status);
                        record.Deserialize();
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        protected abstract int GetStatusOrder(string status);
    }
}