using System.Collections.Generic;

namespace Elements.Common.Presenter.VlcEngine.Caching
{
    public class VlcStreamCacheItem : IVlcStreamCacheItem
    {
        private readonly IVlcStream _stream;
        private readonly ISet<IVlcStreamReceiver> _receivers;
        
        public VlcStreamCacheItem(IVlcStream stream, IVlcStreamReceiver receiver = null)
        {
            _stream = stream;
            _receivers = new HashSet<IVlcStreamReceiver>();
    		
            if (receiver != null)
                _receivers.Add(receiver);
        }
        
        public bool HasAnyReceiver() => _receivers.Count > 0;
        
        public bool TryGetStream(out IVlcStream stream)
        {
            if (_stream is { IsDisposed: false })
            {
                stream = _stream;
                return true;
            }
            
            stream = null;
            return false;
        }
        
        public bool TryAddReceiver(IVlcStreamReceiver receiver) => receiver != null && _receivers.Add(receiver);
        
        public bool TryRemoveReceiver(IVlcStreamReceiver receiver) => receiver != null && _receivers.Remove(receiver);
        
        public void Dispose()
        {
            if (_stream is { IsDisposed: false }) 
                _stream.Dispose();
            
            _receivers.Clear();
        }
    }
}