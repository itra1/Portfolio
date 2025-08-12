using Core.Elements.Windows.Gis.Data;
using Core.Materials.Attributes;
using Elements.FloatingWindow.Presenter.WindowAdapters.WebView;
using Elements.Windows.Gis.Presenter;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Gis
{
    [MaterialData(typeof(GisMaterialData))]
    public class GisPresenterAdapter : WebViewPresenterAdapterBase<GisPresenter>
    {
        
    }
}
