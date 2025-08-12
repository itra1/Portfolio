using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.News.Data
{
    public abstract class NewsDataBase<TNewsItem> : WidgetDataBase, ISelfDeserializable
        where TNewsItem : NewsItemBase
    {
        [MaterialDataPropertyParse("records")]
        public List<TNewsItem> Records { get; set; }
        
        public void Deserialize()
        {
            if (Records != null)
            {
                foreach (var record in Records)
                {
                    var time = record.Time;
                    
                    if (!string.IsNullOrEmpty(time))
                        record.DateTime = System.DateTime.Parse(time);
                }
            }
        }
    }
}