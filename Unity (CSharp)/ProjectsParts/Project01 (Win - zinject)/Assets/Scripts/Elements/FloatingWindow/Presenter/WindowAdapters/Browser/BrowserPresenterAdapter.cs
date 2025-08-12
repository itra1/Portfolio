using Core.Elements.Windows.Browser.Data;
using Core.Materials.Attributes;
using Elements.FloatingWindow.Presenter.WindowAdapters.WebView;
using Elements.Windows.Browser.Presenter;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Browser
{
    [MaterialData(typeof(BrowserMaterialData))]
    public class BrowserPresenterAdapter : WebViewPresenterAdapterBase<BrowserPresenter>
    {
        
    }
}
