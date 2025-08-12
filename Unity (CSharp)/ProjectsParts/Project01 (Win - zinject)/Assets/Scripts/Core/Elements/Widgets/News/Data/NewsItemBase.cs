using Core.Materials.Attributes;

namespace Core.Elements.Widgets.News.Data
{
    public abstract class NewsItemBase
    {
        [MaterialDataPropertyParse("title")]
        public string Title { get; set; }
        
        [MaterialDataPropertyParse("message")]
        public string Message { get; set; }
        
        [MaterialDataPropertyParse("icone")]
        public string Icon { get; set; }
        
        [MaterialDataPropertyParse("image")]
        public string Image { get; set; }
        
        [MaterialDataPropertyParse("time")]
        public string Time { get; set; }
        
        public System.DateTime DateTime { get; set; }
    }
}