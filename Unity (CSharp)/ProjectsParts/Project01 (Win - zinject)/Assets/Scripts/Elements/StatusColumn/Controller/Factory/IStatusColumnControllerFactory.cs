using Core.Elements.StatusColumn.Data;

namespace Elements.StatusColumn.Controller.Factory
{
	public interface IStatusColumnControllerFactory
	{
		IStatusColumnController Create(StatusContentAreaMaterialData areaMaterial);
	}
}