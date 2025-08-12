using Base;
using Core.FileResources;
using Core.Materials.Loading.Loader;
using Core.Materials.Storage;
using Core.Options.Offsets;
using Cysharp.Threading.Tasks;
using Elements.Windows.VideoSplit.Presenter.VideoPlayer.Factory;

namespace UI.SplashScreens.Screensaver.Presenter
{
    public interface IScreensaverPresenter : IVisualAsync, IUnloadable
    {
        bool Active { get; }

        void Initialize(IScreenOffsets screenOffsets,
            IResourceProvider resources,
            IMaterialDataLoader materialLoader,
            IMaterialDataStorage materials,
            IVideoSplitPlayerFactory playerFactory);
        
        UniTaskVoid SetAsync(bool isVisible, string type, ulong? materialId);

        void TogglePlayPause();
    }
}