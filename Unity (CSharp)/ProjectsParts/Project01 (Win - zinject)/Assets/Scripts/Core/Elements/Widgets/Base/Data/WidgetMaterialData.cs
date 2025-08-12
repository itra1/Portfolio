using System.Text;
using Core.Elements.Widgets.Common.Data.Parsing;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Workers.Material;
using Leguar.TotalJSON;

namespace Core.Elements.Widgets.Base.Data
{
	/// <summary>
	/// Устаревшее название - "WidgetsMaterial"
	/// </summary>
	[MaterialModel(MaterialModel.Widget)]
	[MaterialDataLoader("/widgets")]
	[MaterialDataParser(typeof(WidgetMaterialDataParser))]
	[MaterialDataWorker(typeof(WidgetMaterialDataWorker))]
	public class WidgetMaterialData : WindowMaterialData
	{
		[MaterialDataPropertyParse("widgetType"), MaterialDataPropertyUpdate]
		public string WidgetType { get; set; }
		
		[MaterialDataPropertyParse("widgetData"), MaterialDataPropertyUpdate]
		public JSON WidgetDataJson { get; set; }
		
		public WidgetDataBase WidgetData { get; set; }
		
		public WidgetMaterialData() => Model = MaterialModel.Widget;
		
		public override string ToString()
		{
			var buffer = new StringBuilder();
			
			buffer.Append('{');
			buffer.Append($"type: {GetType().Name}, id: {Id}");
			
			if (!string.IsNullOrEmpty(WidgetType))
				buffer.Append($", widgetType: {WidgetType}");
			
			if (!string.IsNullOrEmpty(Name))
				buffer.Append($", name: {Name}");
			
			buffer.Append('}');
			
			return buffer.ToString();
		}
	}
}