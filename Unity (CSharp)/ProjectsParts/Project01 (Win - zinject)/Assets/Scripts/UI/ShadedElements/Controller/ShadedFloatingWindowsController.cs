using System;
using Elements.Common.Presenter.Factory;
using UI.ShadedElements.Controller.Base;
using UI.ShadedElements.Presenter;

namespace UI.ShadedElements.Controller
{
    public class ShadedFloatingWindowsController : ShadedElementsControllerBase<ShadedFloatingWindowsPresenter>,
        IShadedFloatingWindowsController, IDisposable
    {
        public ShadedFloatingWindowsController(IPresenterFactory presenterFactory) : base(presenterFactory) { }
    }
}