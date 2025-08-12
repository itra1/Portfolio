using Base.Presenter;
using Core.Elements.Windows.Camera.Data;
using Core.Materials.Attributes;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.Windows.Camera.Presenter;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Camera
{
	[MaterialData(typeof(CameraMaterialData))]
	public class CameraPresenterAdapter : WindowPresenterAdapterBase<CameraPresenter>, INonRenderedCapable
	{
		public void SetNonRenderedContainer(INonRenderedContainer container) => 
			Adaptee.SetNonRenderedContainer(container);
		
		public override void UpdateContent() => Adaptee.Player.AdjustSize();
	}
}
