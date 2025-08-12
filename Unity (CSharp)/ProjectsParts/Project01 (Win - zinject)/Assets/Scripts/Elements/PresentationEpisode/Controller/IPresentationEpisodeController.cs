using Base.Controller;
using Core.Elements.PresentationEpisode.Data;

namespace Elements.PresentationEpisode.Controller
{
	public interface IPresentationEpisodeController : IController, IPreloadingAsync
	{
		PresentationEpisodeMaterialData Material { get; }
		PresentationEpisodeAreaMaterialData AreaMaterial { get; }
	}
}