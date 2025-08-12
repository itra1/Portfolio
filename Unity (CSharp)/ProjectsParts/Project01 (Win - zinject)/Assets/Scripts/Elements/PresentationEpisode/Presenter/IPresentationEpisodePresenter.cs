using Base.Presenter;
using Core.Elements.PresentationEpisode.Data;

namespace Elements.PresentationEpisode.Presenter
{
	public interface IPresentationEpisodePresenter : IPresenter
	{
		bool SetMaterials(PresentationEpisodeMaterialData material, PresentationEpisodeAreaMaterialData areaMaterial);
	}
}