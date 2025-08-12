using Core.Materials.Data;
using Core.Materials.Storage;
using Elements.Common.Presenter.Factory;
using Elements.Windows.Common.Controller.Factory;
using Preview;
using ScreenStreaming;

namespace Elements.StatusTabItem.Controller.Factory
{
	public class StatusTabItemControllerFactory : IStatusTabItemControllerFactory
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IWindowControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IPreviewProvider _previewProvider;
		private readonly IScreenStreamingController _screenStreaming;
		
		public StatusTabItemControllerFactory(IMaterialDataStorage materials,
			IWindowControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IPreviewProvider previewProvider,
			IScreenStreamingController screenStreaming)
		{
			_materials = materials;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_previewProvider = previewProvider;
			_screenStreaming = screenStreaming;
		}
		
		public IStatusTabItemController Create(ContentAreaMaterialData areaMaterial)
		{
			return new StatusTabItemController(areaMaterial,
				_materials,
				_childControllerFactory,
				_presenterFactory,
				_previewProvider,
				_screenStreaming);
		}
	}
}