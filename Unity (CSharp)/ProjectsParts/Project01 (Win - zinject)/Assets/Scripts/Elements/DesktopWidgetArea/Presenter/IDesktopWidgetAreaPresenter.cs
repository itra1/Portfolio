using Base.Presenter;
using Core.Materials.Data;

namespace Elements.DesktopWidgetArea.Presenter
{
	public interface IDesktopWidgetAreaPresenter : IPresenter, IParentArea
	{
		bool SetMaterial(ContentAreaMaterialData areaMaterial);
		void IdentifyOnlyChild();
	}
}