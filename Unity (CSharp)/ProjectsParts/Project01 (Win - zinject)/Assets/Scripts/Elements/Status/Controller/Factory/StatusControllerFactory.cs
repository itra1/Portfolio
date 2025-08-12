using Core.Elements.Status.Data;
using Core.Materials.Storage;
using Core.Network.Http;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Core.User.Installation;
using Elements.Common.Presenter.Factory;
using Elements.StatusColumn.Controller.Factory;

namespace Elements.Status.Controller.Factory
{
	public class StatusControllerFactory : IStatusControllerFactory
	{
		private readonly IUserInstallation _installation;
		private readonly IApplicationOptions _options;
		private readonly IMaterialDataStorage _materials;
		private readonly IStatusColumnControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IHttpRequest _request;
		private readonly IOutgoingStateController _outgoingState;
		
		public StatusControllerFactory(IUserInstallation installation,
			IApplicationOptions options,
			IMaterialDataStorage materials,
			IStatusColumnControllerFactory childControllerFactory, 
			IPresenterFactory presenterFactory,
			IHttpRequest request,
			IOutgoingStateController outgoingState)
		{
			_installation = installation;
			_options = options;
			_materials = materials;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_request = request;
			_outgoingState = outgoingState;
		}
			
		public IStatusController Create(StatusMaterialData material, StatusAreaMaterialData areaMaterial)
		{
			return new StatusController(material,
				areaMaterial,
				_installation,
				_options,
				_materials,
				_childControllerFactory,
				_presenterFactory,
				_request,
				_outgoingState);
		}
	}
}