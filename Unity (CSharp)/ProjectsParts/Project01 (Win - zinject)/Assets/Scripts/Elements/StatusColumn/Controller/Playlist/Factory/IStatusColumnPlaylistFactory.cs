using Core.Elements.StatusColumn.Data;
using Elements.StatusTabs.Controller;

namespace Elements.StatusColumn.Controller.Playlist.Factory
{
    public interface IStatusColumnPlaylistFactory
    {
        IStatusColumnPlaylist Create(StatusContentMaterialData statusContent, IStatusTabsActionPerformer actionPerformer);
    }
}