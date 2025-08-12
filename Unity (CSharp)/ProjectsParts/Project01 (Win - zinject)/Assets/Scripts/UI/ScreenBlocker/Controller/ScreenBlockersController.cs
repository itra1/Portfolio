using System;
using Core.Options.Offsets;
using UI.Canvas.Presenter;
using UI.ScreenBlocker.Presenter;
using UI.ScreenBlocker.Presenter.Popups.Common.Factory;

namespace UI.ScreenBlocker.Controller
{
    public class ScreenBlockersController : IScreenBlockersController, IDisposable
    {
        private IScreenBlockersPresenter _presenter;
        
        public ScreenBlockersController(IScreenOffsets screenOffsets,
            ICanvasPresenter root,
            IScreenBlockerPopupFactory factory)
        {
            _presenter = root.ScreenBlockers;
            _presenter.Initialize(screenOffsets, factory);
        }
        
        public void Dispose()
        {
            if (_presenter == null)
                return;
            
            _presenter.Unload();
            _presenter = null;
        }
    }
}