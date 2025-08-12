using System;
using com.ootii.Messages;
using Core.App;
using Core.FileResources;
using Core.Materials.Loading.Loader;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Options.Offsets;
using Cysharp.Threading.Tasks;
using Elements.Windows.VideoSplit.Presenter.VideoPlayer.Factory;
using UI.Canvas.Presenter;
using UI.SplashScreens.Screensaver.Presenter;

namespace UI.SplashScreens.Screensaver.Controller
{
    public class ScreensaverController : IScreensaverController, IDisposable
    {
        private readonly IApplicationState _applicationState;
        private readonly IOutgoingStateController _outgoingState;
        
        private IScreensaverPresenter _presenter;
		
        public ScreensaverController(IApplicationState applicationState,
            IScreenOffsets screenOffsets, 
            IResourceProvider resources,
            IMaterialDataLoader materialLoader,
            IMaterialDataStorage materials,
            IVideoSplitPlayerFactory playerFactory,
            IOutgoingStateController outgoingState,
            ICanvasPresenter root)
        {
            _applicationState = applicationState;
            _outgoingState = outgoingState;
            
            _presenter = root.Screensaver;
            _presenter.Initialize(screenOffsets, resources, materialLoader, materials, playerFactory);
            
            MessageDispatcher.AddListener(MessageType.ScreensaverSet, OnScreensaverSet);
            MessageDispatcher.AddListener(MessageType.ScreenLock, OnScreenLocked);
        }

        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.ScreensaverSet, OnScreensaverSet);
            MessageDispatcher.RemoveListener(MessageType.ScreenLock, OnScreenLocked);

            if (_presenter != null)
            {
                _presenter.Unload();
                _presenter = null;
            }
        }

        private void OnScreensaverSet(IMessage message)
        {
            var data = (ScreensaverSet) message.Data;
            
            var isVisible = data.IsVisible;
            var type = data.Type;
            var materialId = data.MaterialId;
            
            _outgoingState.Context.SetScreensaverState(isVisible, type, materialId);
            _outgoingState.PrepareToSendIfAllowed();
            
            _presenter.SetAsync(isVisible, type, materialId);
        }
        
        private void OnScreenLocked(IMessage message)
        {
            if (!_presenter.Active)
                return;
            
            if (_applicationState.IsScreenLocked)
                _presenter.HideAsync(onFinished: _presenter.TogglePlayPause).Forget();
            else
                _presenter.ShowAsync(onStarted: _presenter.TogglePlayPause).Forget();
        }
    }
}