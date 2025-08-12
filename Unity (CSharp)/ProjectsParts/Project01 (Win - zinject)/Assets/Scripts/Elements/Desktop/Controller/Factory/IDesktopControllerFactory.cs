using Core.Elements.Desktop.Data;

namespace Elements.Desktop.Controller.Factory
{
	public interface IDesktopControllerFactory
	{
		IDesktopController Create(DesktopMaterialData material, DesktopAreaMaterialData areaMaterial);
	}
}