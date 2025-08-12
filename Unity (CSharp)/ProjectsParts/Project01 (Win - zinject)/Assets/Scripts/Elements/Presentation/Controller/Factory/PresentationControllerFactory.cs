using Core.Elements.Presentation.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Storage;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Elements.Common.Presenter.Factory;
using Elements.PresentationEpisode.Controller.Factory;

namespace Elements.Presentation.Controller.Factory
{
	public class PresentationControllerFactory : IPresentationControllerFactory
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IPresentationEpisodeControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IOutgoingStateController _outgoingState;
		
		public PresentationControllerFactory(IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IPresentationEpisodeControllerFactory childControllerFactory, 
			IPresenterFactory presenterFactory,
			IOutgoingStateController outgoingState)
		{
			_materials = materials;
			_materialLoader = materialLoader;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_outgoingState = outgoingState;
		}
		
		public IPresentationController Create(PresentationMaterialData material, PresentationAreaMaterialData areaMaterial)
		{
			return new PresentationController(material,
				areaMaterial,
				_materials,
				_materialLoader,
				_childControllerFactory,
				_presenterFactory,
				_outgoingState);
		}
	}
}