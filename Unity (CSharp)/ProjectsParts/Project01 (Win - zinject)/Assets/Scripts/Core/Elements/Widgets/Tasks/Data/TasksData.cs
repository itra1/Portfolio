using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Consts;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Tasks.Data
{
	[WidgetDataTypeKey(WidgetDataTypeKey.Tasks)]
	public class TasksData : WidgetDataBase
	{
		[MaterialDataPropertyParse("create")]
		public int Create { get; set; }
		
		[MaterialDataPropertyParse("active")]
		public int Active { get; set; }

		[MaterialDataPropertyParse("complete")]
		public int Complete { get; set; }

		[MaterialDataPropertyParse("expired")]
		public int Expired { get; set; }
	}
}