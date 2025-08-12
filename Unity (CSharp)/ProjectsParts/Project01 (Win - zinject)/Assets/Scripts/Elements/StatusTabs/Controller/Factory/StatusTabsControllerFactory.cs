using System.Collections.Generic;
using Core.Elements.StatusColumn.Data;
using Core.Materials.Data;
using Core.Network.Http;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Elements.Common.Presenter.Factory;
using Elements.StatusTabItem.Controller.Factory;

namespace Elements.StatusTabs.Controller.Factory
{
	public class StatusTabsControllerFactory : IStatusTabsControllerFactory
	{
		private readonly IApplicationOptions _options;
		private readonly IStatusTabItemControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IHttpRequest _request;
		private readonly IOutgoingStateController _outgoingState;
		
		public StatusTabsControllerFactory(IApplicationOptions options,
			IStatusTabItemControllerFactory childControllerFactory, 
			IPresenterFactory presenterFactory,
			IHttpRequest request,
			IOutgoingStateController outgoingState)
		{
			_options = options;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_request = request;
			_outgoingState = outgoingState;
		}
		
		public IStatusTabsController Create(StatusContentAreaMaterialData areaMaterial, List<ContentAreaMaterialData> areaMaterials) =>
			new StatusTabsController(areaMaterial,
				areaMaterials,
				_options,
				_childControllerFactory,
				_presenterFactory,
				_request,
				_outgoingState);
	}
}