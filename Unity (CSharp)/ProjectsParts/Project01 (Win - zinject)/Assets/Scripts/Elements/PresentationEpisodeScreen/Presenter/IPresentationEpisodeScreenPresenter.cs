using Base.Presenter;
using Core.Materials.Data;

namespace Elements.PresentationEpisodeScreen.Presenter
{
	public interface IPresentationEpisodeScreenPresenter : IPresenter, IParentArea, IFocusable
	{
		bool SetMaterial(ContentAreaMaterialData areaMaterial);
	}
}