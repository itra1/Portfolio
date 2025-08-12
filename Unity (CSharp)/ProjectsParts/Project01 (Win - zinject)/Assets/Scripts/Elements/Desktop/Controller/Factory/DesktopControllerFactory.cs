using Core.Elements.Desktop.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Elements.Common.Presenter.Factory;
using Elements.DesktopWidgetArea.Controller.Factory;
using Preview;

namespace Elements.Desktop.Controller.Factory
{
	public class DesktopControllerFactory : IDesktopControllerFactory
	{
		private readonly IDesktopWidgetAreaControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IOutgoingStateController _outgoingState;
		private readonly IPreviewProvider _previewProvider;
		
		public DesktopControllerFactory(IDesktopWidgetAreaControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IOutgoingStateController outgoingState,
			IPreviewProvider previewProvider)
		{
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_outgoingState = outgoingState;
			_previewProvider = previewProvider;
		}
		
		public IDesktopController Create(DesktopMaterialData material, DesktopAreaMaterialData areaMaterial) => 
			new DesktopController(material,
				areaMaterial,
				_childControllerFactory,
				_presenterFactory,
				_outgoingState,
				_previewProvider);
	}
}