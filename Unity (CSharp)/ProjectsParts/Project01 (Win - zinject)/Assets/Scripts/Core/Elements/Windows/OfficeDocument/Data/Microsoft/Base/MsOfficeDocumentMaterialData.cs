using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Common.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Windows.OfficeDocument.Data.Microsoft.Base
{
    public abstract class MsOfficeDocumentMaterialData : WindowMaterialData
    {
        [MaterialDataPropertyParse("file")]
        public FileItemMaterialData File { get; set; }
    }
}