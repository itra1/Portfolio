using Base.Presenter;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.Windows.Base.Presenter;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.WebView
{
    public abstract class WebViewPresenterAdapterBase<TWindowPresenter> : WindowPresenterAdapterBase<TWindowPresenter>, INonRenderedCapable
        where TWindowPresenter : IWindowPresenter, INonRenderedCapable
    {
        public void SetNonRenderedContainer(INonRenderedContainer container) => 
            Adaptee.SetNonRenderedContainer(container);
    }
}