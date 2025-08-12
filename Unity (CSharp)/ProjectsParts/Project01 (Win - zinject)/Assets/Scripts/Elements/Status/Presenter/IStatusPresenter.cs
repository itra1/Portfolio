using Base.Presenter;
using Core.Elements.Status.Data;

namespace Elements.Status.Presenter
{
	public interface IStatusPresenter : IPresenter
	{
		bool SetMaterial(StatusMaterialData material, StatusAreaMaterialData areaMaterial);
	}
}