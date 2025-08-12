using Core.Elements.PresentationEpisode.Data;
using Core.Materials.Loading.Loader;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Elements.Common.Presenter.Factory;
using Elements.Presentation.Controller.CloneAlias;
using Elements.PresentationEpisodeScreen.Controller.Factory;

namespace Elements.PresentationEpisode.Controller.Factory
{
	public class PresentationEpisodeControllerFactory : IPresentationEpisodeControllerFactory
	{
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IPresentationEpisodeScreenControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IOutgoingStateController _outgoingState;
		
		public PresentationEpisodeControllerFactory(IMaterialDataLoader materialLoader,
			IPresentationEpisodeScreenControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IOutgoingStateController outgoingState)
		{
			_materialLoader = materialLoader;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_outgoingState = outgoingState;
		}
			
		public IPresentationEpisodeController Create(PresentationEpisodeMaterialData material, 
			PresentationEpisodeAreaMaterialData areaMaterial, 
			IPresentationCloneAliasStorage cloneAliasStorage)
		{
			return new PresentationEpisodeController(material,
				areaMaterial,
				_materialLoader,
				cloneAliasStorage,
				_childControllerFactory,
				_presenterFactory,
				_outgoingState);
		}
	}
}