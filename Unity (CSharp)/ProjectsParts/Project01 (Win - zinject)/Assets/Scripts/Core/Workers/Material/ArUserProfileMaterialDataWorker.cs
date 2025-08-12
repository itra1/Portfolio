using Core.Materials.Data;
using Core.Materials.Parsing;
using Zenject;

namespace Core.Workers.Material
{
    public class ArUserProfileMaterialDataWorker : IAfterAddingToStorage
    {
        private IMaterialDataParsingHelper _parsingHelper;
        
        [Inject]
        private void Initialize(IMaterialDataParsingHelper parsingHelper) => _parsingHelper = parsingHelper;
        
        public void PerformActionAfterAddingToStorageOf(MaterialData material)
        {
            if (material is not ArUserProfileMaterialData userProfileMaterial)
                return;
            
            userProfileMaterial.CalendarDataSource =
                _parsingHelper.Parse<CalendarDataSourceMaterialData>(userProfileMaterial.DataJson);
        }
    }
}