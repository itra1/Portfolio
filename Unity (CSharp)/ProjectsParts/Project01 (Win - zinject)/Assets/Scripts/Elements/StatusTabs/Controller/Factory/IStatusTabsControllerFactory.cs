using System.Collections.Generic;
using Core.Elements.StatusColumn.Data;
using Core.Materials.Data;

namespace Elements.StatusTabs.Controller.Factory
{
	public interface IStatusTabsControllerFactory
	{
		IStatusTabsController Create(StatusContentAreaMaterialData areaMaterial, List<ContentAreaMaterialData> areaMaterials);
	}
}