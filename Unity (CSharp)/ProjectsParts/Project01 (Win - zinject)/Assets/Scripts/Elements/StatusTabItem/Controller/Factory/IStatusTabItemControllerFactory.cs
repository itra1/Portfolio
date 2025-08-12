using Core.Materials.Data;

namespace Elements.StatusTabItem.Controller.Factory
{
	public interface IStatusTabItemControllerFactory
	{
		IStatusTabItemController Create(ContentAreaMaterialData areaMaterial);
	}
}