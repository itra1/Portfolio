using Core.Elements.Windows.OfficeDocument.Data.Microsoft;
using Core.Materials.Attributes;
using Elements.FloatingWindow.Presenter.WindowAdapters.OfficeDocument.Microsoft.Base;
using Elements.Windows.OfficeDocument.Presenter.Microsoft;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.OfficeDocument.Microsoft
{
    [MaterialData(typeof(MsWordDocumentMaterialData))]
    public class MsWordDocumentPresenterAdapter : MsOfficeDocumentPresenterAdapterBase<MsWordDocumentPresenter>
    {
        
    }
}