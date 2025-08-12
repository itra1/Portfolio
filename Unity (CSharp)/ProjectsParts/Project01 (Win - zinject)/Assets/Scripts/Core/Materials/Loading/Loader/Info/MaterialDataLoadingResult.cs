using System.Collections.Generic;
using System.Linq;
using Core.Materials.Data;

namespace Core.Materials.Loading.Loader.Info
{
    public readonly struct MaterialDataLoadingResult
    {
        private readonly IReadOnlyList<MaterialData> _materials;
        
        public string ErrorMessage { get; }
        public bool Success => string.IsNullOrEmpty(ErrorMessage) && _materials is { Count: > 0 };
        
        public MaterialDataLoadingResult(IReadOnlyList<MaterialData> materials, string errorMessage)
        {
            _materials = materials;
            ErrorMessage = errorMessage;
        }
        
        public bool TryGetFirstMaterial<TMaterialData>(out TMaterialData material) where TMaterialData : MaterialData
        {
            if (_materials.Count > 0 && _materials.FirstOrDefault(m => m is TMaterialData) is TMaterialData result)
            {
                material = result;
                return true;
            }
            
            material = null;
            return false;
        }
    }
}