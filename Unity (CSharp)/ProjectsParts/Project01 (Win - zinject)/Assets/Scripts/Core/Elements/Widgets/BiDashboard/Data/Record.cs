using Core.Materials.Attributes;

namespace Core.Elements.Widgets.BiDashboard.Data
{
    public class Record
    {
        [MaterialDataPropertyParse("title")]
        public string Title { get; set; }

        [MaterialDataPropertyParse("isWork")]
        public bool IsWork { get; set; }

        [MaterialDataPropertyParse("count")]
        public int Count { get; set; }
    }
}