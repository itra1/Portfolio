using Base;
using Base.Controller;
using Core.Elements.Windows.Base.Data;
using Elements.FloatingWindow.Base;
using Elements.FloatingWindow.Presenter;
using Elements.Windows.Base;

namespace Elements.FloatingWindow.Controller
{
	public interface IFloatingWindowController : IController, IVisible, IPreloadingAsync,
		IWindowMaterialActionPerformer, IFloatingWindowMaterialActionPerformer
	{
		WindowMaterialData Material { get; }
		IFloatingWindowOptions Options { get; }
		
		bool Active { get; }
		
		void Activate();
		void Deactivate();
	}
}
