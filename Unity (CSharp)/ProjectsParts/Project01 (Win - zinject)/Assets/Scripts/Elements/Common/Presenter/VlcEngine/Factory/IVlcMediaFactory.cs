using LibVLCSharp;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public interface IVlcMediaFactory
    {
        Media Create(LibVLC library, string url);
        void Destroy(ref Media media);
    }
}