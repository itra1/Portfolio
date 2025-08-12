using Core.Elements.Presentation.Data;
using Core.Elements.PresentationEpisode.Data;
using Elements.ScreenModes.Controller;

namespace Elements.Presentations.Controller
{
	public interface IPresentationsController : IScreenModeController
	{
		PresentationMaterialData ActivePresentationMaterial { get; }
		PresentationAreaMaterialData ActivePresentationAreaMaterial { get; }
		PresentationEpisodeMaterialData ActivePresentationEpisodeMaterial { get; }
		PresentationEpisodeAreaMaterialData ActivePresentationAreaEpisodeMaterial { get; }
	}
}