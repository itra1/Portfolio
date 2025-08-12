using Core.Elements.Windows.Browser.Data;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Elements.Windows.Cnp.Data
{
	/// <summary>
	/// Устаревшее название - "CnpMaterial"
	/// </summary>
	[MaterialModel(MaterialModel.Cnp)]
	[MaterialDataLoader("/kcsystems")]
	public class CnpMaterialData : BrowserMaterialData
	{
		public CnpMaterialData()
		{
			Model = MaterialModel.Cnp;
			Category = MaterialCategory.Cnp;
		}
	}
}