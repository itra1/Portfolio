using Core.Elements.Windows.Base.Data;

namespace Elements.FloatingWindow.Controller.Factory
{
	public interface IFloatingWindowControllerFactory
	{
		IFloatingWindowController Create(WindowMaterialData material, bool isAdaptiveSizeRequired);
	}
}
