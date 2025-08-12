namespace Elements.Common.Presenter.VlcEngine
{
    public interface IVlcPlayerState
    {
        bool IsInitialized { get; }
        bool IsPlaying { get; }
        bool IsDisplayed { get; }
        bool IsDisposed { get; }
    }
}