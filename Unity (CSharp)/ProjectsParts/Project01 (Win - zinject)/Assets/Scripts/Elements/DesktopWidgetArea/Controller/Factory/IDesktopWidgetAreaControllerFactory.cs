using Core.Materials.Data;

namespace Elements.DesktopWidgetArea.Controller.Factory
{
	public interface IDesktopWidgetAreaControllerFactory
	{
		IDesktopWidgetAreaController Create(ContentAreaMaterialData areaMaterial);
	}
}