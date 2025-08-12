using System.Collections.Generic;
using System.Threading;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Cysharp.Threading.Tasks;
using Debug = Core.Logging.Debug;

namespace Elements.StatusColumn.Controller.Helper
{
    public class StatusColumnMaterialsHelper : IStatusColumnMaterialsHelper
    {
	    private readonly IMaterialDataStorage _materials;
	    private readonly IMaterialDataLoader _materialLoader;

	    public StatusColumnMaterialsHelper(IMaterialDataStorage materials, IMaterialDataLoader materialLoader)
	    {
		    _materials = materials;
		    _materialLoader = materialLoader;
	    }
	    
	    public async UniTask<bool> TryFindOrderedMaterialsAsync(IList<ulong> contentOrder,
		    IList<ContentAreaMaterialData> orderedAreaMaterials,
		    IList<WindowMaterialData> orderedMaterials,
			CancellationToken cancellationToken)
	    {
		    if (contentOrder.Count == 0)
			    return true;
		    
		    if (orderedAreaMaterials.Count > 0)
			    orderedAreaMaterials.Clear();
		    
		    if (orderedMaterials.Count > 0)
			    orderedMaterials.Clear();
		    
		    for (var i = 0; i < contentOrder.Count; i++)
			{
				var (areaMaterial, material) = await FindMaterialsAsync(contentOrder[i], cancellationToken);
				
				if (areaMaterial == null || material == null)
					return false;
				
				orderedAreaMaterials.Add(areaMaterial);
				orderedMaterials.Add(material);
				
				if (areaMaterial.Order == null)
					areaMaterial.Order = i + 1;
			}
		    
		    if (contentOrder.Count != orderedAreaMaterials.Count || contentOrder.Count != orderedMaterials.Count)
		    {
			    Debug.LogError("A mismatch was detected in the count of elements of the ordered lists");
			    return false;
		    }
		    
		    SortListsInOrder(contentOrder, orderedAreaMaterials, orderedMaterials);
		    return true;
	    }
		
        public async UniTask<bool> TryFindOrderedMaterialsAsync(IList<ulong> contentOrder,
	        IList<WindowMaterialData> orderedMaterials,
			CancellationToken cancellationToken)
		{
			if (contentOrder.Count == 0)
				return true;
			
			var orderedAreaMaterials = new List<ContentAreaMaterialData>();
			
			if (orderedMaterials.Count > 0)
				orderedMaterials.Clear();
			
			for (var i = 0; i < contentOrder.Count; i++)
			{
				var (areaMaterial, material) = await FindMaterialsAsync(contentOrder[i], cancellationToken);
				
				if (areaMaterial == null || material == null)
					return false;
				
				orderedAreaMaterials.Add(areaMaterial);
				orderedMaterials.Add(material);
				
				if (areaMaterial.Order == null)
					areaMaterial.Order = i + 1;
			}
			
			if (contentOrder.Count != orderedAreaMaterials.Count ||contentOrder.Count != orderedMaterials.Count)
			{
				Debug.LogError("A mismatch was detected in the count of elements of the ordered lists");
				return false;
			}
			
			SortListsInOrder(contentOrder, orderedAreaMaterials, orderedMaterials);
			return true;
		}
        
		public async UniTask<WindowMaterialData> FindWindowMaterialAsync(ulong materialId,
			CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
	        
			var windowMaterial = GetMaterial<WindowMaterialData>(materialId) ?? await TryLoadWindowMaterialAsync(materialId, cancellationToken);
			
			if (windowMaterial != null) 
				return windowMaterial;
	        
			Debug.LogError($"Failed to get window material by id {materialId}");
			return null;
		}
        
        private ContentAreaMaterialData FindAreaMaterial(ulong areaMaterialId)
        {
	        var areaMaterial = GetMaterial<ContentAreaMaterialData>(areaMaterialId);
	        
	        if (areaMaterial != null) 
		        return areaMaterial;
	        
	        Debug.LogError($"Content area material id \"{areaMaterialId}\" is missing from the material data storage");
	        return null;
        }
        
        public TMaterial GetMaterial<TMaterial>(ulong materialId) where TMaterial : MaterialData => _materials.Get<TMaterial>(materialId);
		
        private async UniTask<(ContentAreaMaterialData, WindowMaterialData)> FindMaterialsAsync(ulong areaMaterialId,
	        CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			var areaMaterial = FindAreaMaterial(areaMaterialId);
            
			if (areaMaterial == null)
				return default;
            
			var materialId = areaMaterial.MaterialId;
            
			if (materialId == null)
			{
				Debug.LogError($"Missing material id for content area material with id {areaMaterial.Id}");
				return (null, null);
			}
			
			var windowMaterial = await FindWindowMaterialAsync(materialId.Value, cancellationToken);
			
			return windowMaterial != null ? (areaMaterial, windowMaterial) : default;
		}
		
        private async UniTask<WindowMaterialData> TryLoadWindowMaterialAsync(ulong materialId,
	        CancellationToken cancellationToken)
		{
			var info = new MaterialDataLoadingInfo(typeof(WindowMaterialData), materialId);
			var result = await _materialLoader.LoadAsync(info);
			
			cancellationToken.ThrowIfCancellationRequested();
			
			if (!result.Success)
			{
				Debug.LogError($"Failed to load window material by id {materialId}");
				return null;
			}
			
			if (!result.TryGetFirstMaterial<WindowMaterialData>(out var material))
			{
				Debug.LogError($"No window material was found in the loaded list of materials by requested material id {materialId}");
				return null;
			}
			
			return material;
		}
        
		private void SortListsInOrder<TWindowMaterialElement>(IList<ulong> contentOrder,
			IList<ContentAreaMaterialData> orderedAreaMaterials,
			IList<TWindowMaterialElement> orderedMaterials)
		{
			for (var i = orderedAreaMaterials.Count - 1; i >= 0; i--)
			{
				var minOrderValueIndex = i;
				
				for (var j = i - 1; j >= 0; j--)
				{
					if (orderedAreaMaterials[j].Order <= orderedAreaMaterials[minOrderValueIndex].Order) 
						minOrderValueIndex = j;
				}
				
				var areaMaterial = orderedAreaMaterials[minOrderValueIndex];
				var material = orderedMaterials[minOrderValueIndex];
				
				contentOrder.RemoveAt(minOrderValueIndex);
				orderedAreaMaterials.RemoveAt(minOrderValueIndex);
				orderedMaterials.RemoveAt(minOrderValueIndex);
				
				orderedAreaMaterials.Add(areaMaterial);
				orderedMaterials.Add(material);
				contentOrder.Add(areaMaterial.Id);
			}
		}
    }
}