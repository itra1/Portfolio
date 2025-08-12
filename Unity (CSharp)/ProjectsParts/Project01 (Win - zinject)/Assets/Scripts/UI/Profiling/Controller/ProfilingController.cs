using System;
using com.ootii.Messages;
using Core.Messages;
using Core.Options;
using UI.Canvas.Presenter;
using UI.Profiling.Presenter.Base;

namespace UI.Profiling.Controller
{
    public class ProfilingController : IProfilingController, IDisposable
    {
        private readonly IApplicationOptions _options;
        
        private IProfilerItemPresenter _fpsCounter;
        
        public ProfilingController(IApplicationOptions options, ICanvasPresenter root)
        {
            _options = options;
            _fpsCounter = root.FpsCounter;
            
            MessageDispatcher.AddListener(MessageType.AppInitialize, OnApplicationInitialized);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            if (_fpsCounter != null)
            {
                if (_fpsCounter.Active)
                    _fpsCounter.Deactivate();
                
                _fpsCounter = null;
            }
        }
        
        private void OnApplicationInitialized(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppInitialize, OnApplicationInitialized);
            
            if (_options.IsFpsCounterEnabled && !_fpsCounter.Active)
                _fpsCounter.Activate();
        }
    }
}