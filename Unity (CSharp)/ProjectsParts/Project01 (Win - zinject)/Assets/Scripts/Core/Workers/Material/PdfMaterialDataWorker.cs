using Core.Elements.Windows.Common.Data.Consts;
using Core.Elements.Windows.Pdf.Data;
using Core.Materials.Data;

namespace Core.Workers.Material
{
    public class PdfMaterialDataWorker : WindowMaterialDataWorker
    {
        public override void PerformActionAfterAddingToStorageOf(MaterialData material)
        {
            base.PerformActionAfterAddingToStorageOf(material);
            
            if (material is PdfMaterialData pdfMaterial)
                pdfMaterial.MaterialType = WindowMaterialType.Pdf;
        }
    }
}