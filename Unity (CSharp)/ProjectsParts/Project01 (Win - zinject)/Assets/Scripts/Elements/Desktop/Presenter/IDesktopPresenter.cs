using Base.Presenter;
using Core.Elements.Desktop.Data;
using Preview;

namespace Elements.Desktop.Presenter
{
	public interface IDesktopPresenter : IPresenter
	{
		void Initialize(IPreviewState previewState);
		
		bool SetMaterials(DesktopMaterialData material, DesktopAreaMaterialData areaMaterial);
	}
}