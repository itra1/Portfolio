using Core.Elements.StatusColumn.Data;
using Core.Materials.Storage;
using Core.Network.Http;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options;
using Elements.StatusTabs.Controller;

namespace Elements.StatusColumn.Controller.Playlist.Factory
{
    public class StatusColumnPlaylistFactory : IStatusColumnPlaylistFactory
    {
        private readonly IApplicationOptions _options;
        private readonly IHttpRequestAsync _requestAsync;
        private readonly IMaterialDataStorage _materials;
        private readonly IOutgoingStateController _outgoingState;
        
        public StatusColumnPlaylistFactory(IApplicationOptions options,
            IHttpRequestAsync requestAsync,
            IMaterialDataStorage materials,
            IOutgoingStateController outgoingState)
        {
            _options = options;
            _requestAsync = requestAsync;
            _materials = materials;
            _outgoingState = outgoingState;
        }
        
        public IStatusColumnPlaylist Create(StatusContentMaterialData statusContent, IStatusTabsActionPerformer actionPerformer) => 
            new StatusColumnPlaylist(statusContent, _options, _requestAsync, _materials, _outgoingState, actionPerformer);
    }
}