using Core.Elements.Windows.Common.Data.Parsing;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Core.Workers.Material;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.Base.Data
{
	/// <summary>
	/// Устаревшее название - "WindowMaterial"
	/// </summary>
	[MaterialDataLoader("/materials")]
	[MaterialDataParser(typeof(WindowMaterialDataParser))]
	[MaterialDataWorker(typeof(WindowMaterialDataWorker))]
	public abstract class WindowMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("state")] 
		public JArray StatesJson { get; set; }
		
		public WindowState[] States { get; set; }
	}
}