using System;
using com.ootii.Messages;
using Core.Messages;
using Core.Options;
using Elements.FloatingWindows.Controller;
using Elements.ScreenModes.Controller;
using UI.Canvas.Presenter;
using UI.ShadedElements.Controller;
using Zenject;

namespace UI.Elements.Controller
{
    public class ElementsController : IElementsController, IDisposable
    {
        private readonly DiContainer _container;
        private readonly IApplicationOptions _options;
        private readonly ICanvasPresenter _root;
        private readonly IScreenModesController _screenModes;
        private readonly IShadedScreenModesController _shadedScreenModes;
		
        private IFloatingWindowsController _floatingWindows;
        private IShadedFloatingWindowsController _shadedFloatingWindows;
        
        public ElementsController(DiContainer container,
            IApplicationOptions options,
            ICanvasPresenter root,
            IScreenModesController screenModes,
            IShadedScreenModesController shadedScreenModes)
        {
            _container = container;
            _options = options;
            _root = root;
            _screenModes = screenModes;
            _shadedScreenModes = shadedScreenModes;
            
            MessageDispatcher.AddListener(MessageType.AppStart, OnApplicationStarted);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);

            _floatingWindows = null;
            _shadedFloatingWindows = null;
        }
        
        private void PreloadAllChildren()
        {
            var content = _root.Elements;
			
            _screenModes.Preload(content);
            _shadedScreenModes.Preload(content);
            
            _floatingWindows?.Preload(content);
            _shadedFloatingWindows?.Preload(content);
        }
        
        private void OnApplicationStarted(IMessage message)
        {
            MessageDispatcher.RemoveListener(MessageType.AppStart, OnApplicationStarted);

            if (!_options.IsSumAdaptiveModeActive)
            {
                _floatingWindows = _container.Resolve<IFloatingWindowsController>();
                _shadedFloatingWindows = _container.Resolve<IShadedFloatingWindowsController>();
            }
			
            PreloadAllChildren();
        }
    }
}