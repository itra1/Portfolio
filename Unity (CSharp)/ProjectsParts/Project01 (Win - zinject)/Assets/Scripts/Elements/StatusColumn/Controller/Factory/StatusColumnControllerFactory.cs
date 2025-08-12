using Core.Elements.StatusColumn.Data;
using Core.Options;
using Elements.Common.Presenter.Factory;
using Elements.StatusColumn.Controller.Helper;
using Elements.StatusColumn.Controller.Playlist.Factory;
using Elements.StatusTabItem.Controller.Buffer;
using Elements.StatusTabs.Controller.Factory;

namespace Elements.StatusColumn.Controller.Factory
{
	public class StatusColumnControllerFactory : IStatusColumnControllerFactory
	{
		private readonly IApplicationOptions _options;
		private readonly IStatusColumnMaterialsHelper _helper;
		private readonly IStatusTabItemDragAndDropBuffer _dragAndDropBuffer;
		private readonly IStatusTabsControllerFactory _tabsControllerFactory;
		private readonly IStatusColumnPlaylistFactory _playlistFactory;
		private readonly IPresenterFactory _presenterFactory;
		
		public StatusColumnControllerFactory(IApplicationOptions options,
			IStatusColumnMaterialsHelper helper,
			IStatusTabItemDragAndDropBuffer dragAndDropBuffer,
			IStatusTabsControllerFactory tabsControllerFactory,
			IStatusColumnPlaylistFactory playlistFactory,
			IPresenterFactory presenterFactory)
		{
			_options = options;
			_helper = helper;
			_dragAndDropBuffer = dragAndDropBuffer;
			_tabsControllerFactory = tabsControllerFactory;
			_playlistFactory = playlistFactory;
			_presenterFactory = presenterFactory;
		}
        
		public IStatusColumnController Create(StatusContentAreaMaterialData areaMaterial)
		{
			return new StatusColumnController(areaMaterial,
				_options,
				_helper,
				_dragAndDropBuffer,
				_tabsControllerFactory,
				_playlistFactory,
				_presenterFactory);
		}
	}
}