using Base.Controller;
using Core.Elements.Desktop.Data;

namespace Elements.Desktop.Controller
{
	public interface IDesktopController : IController, IPreloadingAsync
	{
		DesktopMaterialData Material { get; }
		DesktopAreaMaterialData AreaMaterial { get; }
	}
}