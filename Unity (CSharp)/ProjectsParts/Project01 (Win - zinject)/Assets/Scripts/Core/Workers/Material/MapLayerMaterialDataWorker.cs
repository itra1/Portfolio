using Core.Elements.Widgets.Map.Data;
using Core.Materials.Data;
using Core.Materials.Storage;
using Zenject;

namespace Core.Workers.Material
{
    public class MapLayerMaterialDataWorker : IAfterAddingToStorage
    {
        private IMaterialDataStorage _materials;

        [Inject]
        private void Initialize(IMaterialDataStorage materials) => _materials = materials;
        
        public void PerformActionAfterAddingToStorageOf(MaterialData material)
        {
            if (material is not MapLayerMaterialData mapLayerMaterial)
                return;
            
            var locationIds = mapLayerMaterial.LocationIds;
            var locations = mapLayerMaterial.Locations;
            
            foreach (var locationId in locationIds)
            {
                var location = _materials.Get<MapLocationMaterialData>(locationId);
                
                if (location == null)
                    continue;
                
                locations.Add(location);
            }
        }
    }
}