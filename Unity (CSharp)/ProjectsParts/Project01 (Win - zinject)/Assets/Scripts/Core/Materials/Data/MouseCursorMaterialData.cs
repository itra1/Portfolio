using Core.Materials.Attributes;
using Core.Workers.Material;
using Leguar.TotalJSON;

namespace Core.Materials.Data
{
    [MaterialDataLoader("/cursor")]
    [MaterialDataWorker(typeof(MouseCursorMaterialDataWorker))]
    public class MouseCursorMaterialData : MaterialData
    {
        [MaterialDataPropertyParse("image")] 
        public JSON RawImageData;
    }
}