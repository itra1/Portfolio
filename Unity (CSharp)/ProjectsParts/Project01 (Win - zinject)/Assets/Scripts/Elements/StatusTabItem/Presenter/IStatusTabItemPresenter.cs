using Base.Presenter;
using Core.Materials.Data;

namespace Elements.StatusTabItem.Presenter
{
	public interface IStatusTabItemPresenter : IPresenter, IParentArea
	{
		bool SetMaterial(ContentAreaMaterialData areaMaterial);
	}
}