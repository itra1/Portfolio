using Elements.Common.Presenter.VlcEngine.Textures;
using LibVLCSharp;

namespace Elements.Common.Presenter.VlcEngine.Factory
{
    public interface IVlcPlayerTexturesFactory
    {
        bool TryCreate(MediaPlayer mediaPlayer, out ITexturesBuffer buffer);
        void Destroy(ref ITexturesBuffer buffer);
    }
}