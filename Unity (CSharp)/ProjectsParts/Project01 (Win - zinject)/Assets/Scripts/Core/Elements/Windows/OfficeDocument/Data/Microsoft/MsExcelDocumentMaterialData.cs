using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.OfficeDocument.Data.Microsoft.Base;
using Core.Materials.Consts;

namespace Core.Elements.Windows.OfficeDocument.Data.Microsoft
{
    [WindowState(typeof(MsExcelDocumentState))]
    public class MsExcelDocumentMaterialData : MsOfficeDocumentMaterialData
    {
        public MsExcelDocumentMaterialData() => Model = MaterialModel.MsExcelDocument;
    }
}