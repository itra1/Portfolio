using Core.Elements.Windows.Cnp.Data;
using Core.Materials.Attributes;
using Elements.FloatingWindow.Presenter.WindowAdapters.WebView;
using Elements.Windows.Cnp.Presenter;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Cnp
{
    [MaterialData(typeof(CnpMaterialData))]
    public class CnpPresenterAdapter : WebViewPresenterAdapterBase<CnpPresenter>
    {
        
    }
}
