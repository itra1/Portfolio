using Core.Materials.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Storage;
using Elements.Common.Presenter.Factory;
using Elements.Presentation.Controller.CloneAlias;
using Elements.Windows.Common.Controller.Factory;
using Preview;
using ScreenStreaming;

namespace Elements.PresentationEpisodeScreen.Controller.Factory
{
	public class PresentationEpisodeScreenControllerFactory : IPresentationEpisodeScreenControllerFactory
	{
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IMaterialDataStorage _materials;
		private readonly IWindowControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IPreviewProvider _previewProvider;
		private readonly IScreenStreamingController _streamingController;

		public PresentationEpisodeScreenControllerFactory(IMaterialDataLoader materialLoader,
			IMaterialDataStorage materials,
			IWindowControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IPreviewProvider previewProvider,
			IScreenStreamingController streamingController)
		{
			_materialLoader = materialLoader;
			_materials = materials;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_previewProvider = previewProvider;
			_streamingController = streamingController;
		}

		public IPresentationEpisodeScreenController Create(ulong? presentationId,
			ContentAreaMaterialData areaMaterial, 
			IPresentationCloneAliasStorage cloneAliasStorage)
		{
			return new PresentationEpisodeScreenController(presentationId,
				areaMaterial,
				_materialLoader,
				_materials,
				cloneAliasStorage,
				_childControllerFactory,
				_presenterFactory,
				_previewProvider,
				_streamingController);
		}
	}
}