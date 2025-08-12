using Core.Materials.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Storage;
using Elements.Common.Presenter.Factory;
using Elements.Widgets.Common.Controller.Factory;

namespace Elements.DesktopWidgetArea.Controller.Factory
{
	public class DesktopWidgetAreaControllerFactory : IDesktopWidgetAreaControllerFactory
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IWidgetControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		
		public DesktopWidgetAreaControllerFactory(IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IWidgetControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory)
		{
			_materials = materials;
			_materialLoader = materialLoader;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
		}
		
		public IDesktopWidgetAreaController Create(ContentAreaMaterialData areaMaterial)
		{
			return new DesktopWidgetAreaController(areaMaterial,
				_materials,
				_materialLoader,
				_childControllerFactory,
				_presenterFactory);
		}
	}
}