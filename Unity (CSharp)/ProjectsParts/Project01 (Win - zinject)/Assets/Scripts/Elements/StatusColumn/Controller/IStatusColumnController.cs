using Base.Controller;
using Core.Elements.StatusColumn.Data;

namespace Elements.StatusColumn.Controller
{
	public interface IStatusColumnController : IController, IPreloadingAsync, IStatusColumnPlaylistContainer
	{
		StatusContentAreaMaterialData AreaMaterial { get; }
	}
}