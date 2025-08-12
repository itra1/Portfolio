using Base;
using Base.Presenter;
using Elements.FloatingWindow.Base;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.Windows.Base;

namespace Elements.FloatingWindow.Presenter
{
	public interface IFloatingWindowPresenter : IPresenter, IPreloadableAsync, IFloatingWindowOptions, IFocusable,
		IWindowMaterialActionPerformer, IFloatingWindowMaterialActionPerformer, IWindowStateSetter
	{
		bool Active { get; }
		
		void AllowAdaptiveSize();
		bool SetWindowAdapter(IWindowPresenterAdapter windowAdapter);
		
		void Activate();
		void Deactivate();
	}
}
