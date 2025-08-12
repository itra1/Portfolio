using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.OfficeDocument.Data.Microsoft.Base;
using Core.Materials.Consts;

namespace Core.Elements.Windows.OfficeDocument.Data.Microsoft
{
    [WindowState(typeof(MsWordDocumentState))]
    public class MsWordDocumentMaterialData : MsOfficeDocumentMaterialData
    {
        public MsWordDocumentMaterialData() => Model = MaterialModel.MsWordDocument;
    }
}