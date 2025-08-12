using System;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using UI.ShadedElements.Controller.Base;
using UI.ShadedElements.Presenter;
using UI.ShadedElements.Presenter.Targets.Base;

namespace UI.ShadedElements.Controller
{
    public class ShadedScreenModesController : ShadedElementsControllerBase<ShadedScreenModesPresenter>, 
        IShadedScreenModesController, IDisposable
    {
        public ShadedScreenModesController(IPresenterFactory presenterFactory) : base(presenterFactory) { }
        
        public bool IsMaximized(IMaximizable target) => Presenter != null && Presenter.IsMaximized(target);

        public bool Maximize(IMaximizable target, Action onMaximized = null, Func<bool, UniTask> onSizeRestoredAsync = null) => 
            Presenter != null && Presenter.Maximize(target, onMaximized, onSizeRestoredAsync);
        
        public bool RestoreSize(IMaximizable target) => Presenter != null && Presenter.RestoreSize(target);
    }
}