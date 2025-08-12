using Core.Elements.Status.Data;

namespace Elements.Status.Controller.Factory
{
	public interface IStatusControllerFactory
	{
		IStatusController Create(StatusMaterialData material, StatusAreaMaterialData areaMaterial);
	}
}