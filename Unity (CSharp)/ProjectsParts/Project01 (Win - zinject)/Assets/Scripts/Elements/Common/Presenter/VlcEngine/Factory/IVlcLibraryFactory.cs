using LibVLCSharp;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public interface IVlcLibraryFactory
    {
        LibVLC Create();
        void Destroy(ref LibVLC library);
    }
}