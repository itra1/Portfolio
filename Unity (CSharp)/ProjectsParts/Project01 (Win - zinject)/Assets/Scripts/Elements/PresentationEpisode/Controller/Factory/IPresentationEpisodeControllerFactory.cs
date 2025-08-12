using Core.Elements.PresentationEpisode.Data;
using Elements.Presentation.Controller.CloneAlias;

namespace Elements.PresentationEpisode.Controller.Factory
{
	public interface IPresentationEpisodeControllerFactory
	{
		IPresentationEpisodeController Create(PresentationEpisodeMaterialData material,
			PresentationEpisodeAreaMaterialData areaMaterial,
			IPresentationCloneAliasStorage cloneAliasStorage);
	}
}