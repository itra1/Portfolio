using Base.Presenter;
using Core.Elements.StatusColumn.Data;

namespace Elements.StatusColumn.Presenter
{
	public interface IStatusColumnPresenter : IPresenter, IFocusable, IStatusHeaderTabsContainer
	{
		bool SetMaterial(StatusContentAreaMaterialData areaMaterial);
	}
}