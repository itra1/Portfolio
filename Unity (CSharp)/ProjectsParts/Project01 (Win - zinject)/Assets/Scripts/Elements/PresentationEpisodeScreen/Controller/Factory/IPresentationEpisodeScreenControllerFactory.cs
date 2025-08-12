using Core.Materials.Data;
using Elements.Presentation.Controller.CloneAlias;

namespace Elements.PresentationEpisodeScreen.Controller.Factory
{
	public interface IPresentationEpisodeScreenControllerFactory
	{
		IPresentationEpisodeScreenController Create(ulong? presentationId,
			ContentAreaMaterialData areaMaterial, 
			IPresentationCloneAliasStorage cloneAliasStorage);
	}
}