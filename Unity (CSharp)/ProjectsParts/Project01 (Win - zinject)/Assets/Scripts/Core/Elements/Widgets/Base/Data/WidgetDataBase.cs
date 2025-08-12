using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Base.Data
{
	public abstract class WidgetDataBase
	{
		[MaterialDataPropertyParse("size")] 
		public int[] Size { get; set; }

		[MaterialDataPropertyParse("type")]
		public string Type { get; set; }
	}
}