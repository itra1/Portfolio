namespace Elements.Common.Presenter.VlcEngine.Caching
{
    public interface IVlcStreamCache
    {
        bool Contains(ulong id);
        bool TryGet(ulong id, IVlcStreamReceiver receiver, out IVlcStream stream);
        bool TryRemove(ulong id, IVlcStreamReceiver receiver);
    }
}