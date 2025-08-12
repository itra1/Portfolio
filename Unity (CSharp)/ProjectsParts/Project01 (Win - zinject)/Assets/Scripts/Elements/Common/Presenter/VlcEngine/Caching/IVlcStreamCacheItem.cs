using System;

namespace Elements.Common.Presenter.VlcEngine.Caching
{
    public interface IVlcStreamCacheItem : IDisposable
    {
        bool HasAnyReceiver();
        bool TryGetStream(out IVlcStream stream);
        bool TryAddReceiver(IVlcStreamReceiver receiver);
        bool TryRemoveReceiver(IVlcStreamReceiver receiver);
    }
}