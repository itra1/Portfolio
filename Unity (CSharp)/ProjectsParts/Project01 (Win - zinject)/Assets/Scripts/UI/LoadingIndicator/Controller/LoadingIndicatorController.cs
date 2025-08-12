using System;
using com.ootii.Messages;
using Core.Messages;
using Core.Options;
using UI.Canvas.Presenter;
using UI.LoadingIndicator.Presenter;

namespace UI.LoadingIndicator.Controller
{
    public class LoadingIndicatorController: ILoadingIndicatorController, IDisposable
    {
        private readonly IApplicationOptions _options;
        
        private ILoadingIndicatorPresenter _presenter;
        
        public LoadingIndicatorController(IApplicationOptions options, ICanvasPresenter root)
        {
            _options = options;
            
            _presenter = root.LoadingIndicator;
            _presenter.Initialize();
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            if (_presenter != null)
            {
                if (_presenter.Active)
                    _presenter.Deactivate();
                
                _presenter.Unload();
                _presenter = null;
            }
        }
        
        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            if (_options.IsLoadingIndicatorEnabled && !_presenter.Active)
                _presenter.Activate();
        }
    }
}