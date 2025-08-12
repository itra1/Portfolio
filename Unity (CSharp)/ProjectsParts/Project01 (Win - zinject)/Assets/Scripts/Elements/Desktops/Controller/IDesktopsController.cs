using Core.Elements.Desktop.Data;
using Elements.ScreenModes.Controller;

namespace Elements.Desktops.Controller
{
	public interface IDesktopsController : IScreenModeController
	{
		DesktopMaterialData ActiveDesktopMaterial { get; }
		DesktopAreaMaterialData ActiveDesktopAreaMaterial { get; }
	}
}