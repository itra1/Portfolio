using System.Collections.Generic;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Attributes;

namespace Core.Elements.Widgets.Map.Data
{
    public abstract class MapDataBase : WidgetDataBase, ISelfDeserializable
    {
        [MaterialDataPropertyParse("background")]
        public ulong BackgroundId { get; set; }
        
        [MaterialDataPropertyParse("dataLayer")]
        public ulong? LayerDataId { get; set; }
        
        [MaterialDataPropertyParse("selectOCTMO")]
        public string SelectedOktmo { get; set; }
        
        [MaterialDataPropertyParse("focusSelected")]
        public bool FocusSelected { get; set; }
        
        [MaterialDataPropertyParse("extendDataId")]
        public ulong? ExtendedLayerDataId { get; set; }
        
        [MaterialDataPropertyParse("extendData")]
        public List<ExtendedLayerData> ExtendedLayerDataList { get; set; }
		
        public MapTextureMaterialData MapTextureData { get; set; }
		
        public MapLayerMaterialData MapLayerData { get; set; }
        
        public void Deserialize()
        {
            foreach (var extendedLayerData in ExtendedLayerDataList)
            {
                extendedLayerData.Deserialize();
                
                foreach (var region in extendedLayerData.Data)
                {
                    region.Oktmo = region.Oktmo.Trim();
                    region.Title = region.Title.Trim();
                }
            }
        }
    }
}