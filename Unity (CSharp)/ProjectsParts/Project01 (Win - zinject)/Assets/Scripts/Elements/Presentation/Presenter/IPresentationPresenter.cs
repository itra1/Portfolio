using Base.Presenter;
using Core.Elements.Presentation.Data;

namespace Elements.Presentation.Presenter
{
	public interface IPresentationPresenter : IPresenter
	{
		bool SetMaterials(PresentationMaterialData material, PresentationAreaMaterialData areaMaterial);
	}
}