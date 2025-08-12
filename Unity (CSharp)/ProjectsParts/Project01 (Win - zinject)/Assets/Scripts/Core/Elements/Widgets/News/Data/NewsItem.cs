using Core.Materials.Attributes;

namespace Core.Elements.Widgets.News.Data
{
    public class NewsItem : NewsItemBase
    {
        [MaterialDataPropertyParse("source")]
        public string Source { get; set; }
    }
}