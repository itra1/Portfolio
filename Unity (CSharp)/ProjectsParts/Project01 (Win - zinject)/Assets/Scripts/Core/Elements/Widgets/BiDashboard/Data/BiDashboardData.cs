using System.Collections.Generic;
using Core.Elements.Widgets.Base.Attributes;
using Core.Elements.Widgets.Base.Consts;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.BiDashboard.Data
{
	/// <summary>
	/// Устаревшее название - "DashbordProjectData"
	/// </summary>
	[WidgetDataTypeKey(WidgetDataTypeKey.BiDashboard)]
	public class BiDashboardData : WidgetDataBase
	{
		[MaterialDataPropertyParse("work")] 
		public int Work { get; set; }

		[MaterialDataPropertyParse("noWork")]
		public int NoWork { get; set; }

		[MaterialDataPropertyParse("records")]
		public List<Record> Records { get; set; }
	}
}