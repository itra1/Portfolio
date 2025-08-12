using System.Linq;
using Core.Elements.Windows.VideoSplit.Data;
using Core.Materials.Data;

namespace Core.Workers.Material
{
    public class VideoSplitMaterialDataWorker : WindowMaterialDataWorker
    {
        public override void PerformActionAfterAddingToStorageOf(MaterialData material)
        {
            base.PerformActionAfterAddingToStorageOf(material);
            
            if (material is not VideoSplitMaterialData videoSplitMaterial)
                return;
            
            videoSplitMaterial.Files = videoSplitMaterial.Files.OrderBy(file => file.Order).ToList();
        }
    }
}