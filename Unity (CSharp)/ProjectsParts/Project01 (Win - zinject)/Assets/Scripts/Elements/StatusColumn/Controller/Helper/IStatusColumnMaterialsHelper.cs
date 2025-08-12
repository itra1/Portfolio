using System.Collections.Generic;
using System.Threading;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;

namespace Elements.StatusColumn.Controller.Helper
{
    public interface IStatusColumnMaterialsHelper
    {
	    UniTask<bool> TryFindOrderedMaterialsAsync(IList<ulong> contentOrder,
		    IList<ContentAreaMaterialData> orderedAreaMaterials,
		    IList<WindowMaterialData> orderedMaterials,
		    CancellationToken cancellationToken);
	    
	    UniTask<bool> TryFindOrderedMaterialsAsync(IList<ulong> contentOrder,
		    IList<WindowMaterialData> orderedMaterials,
		    CancellationToken cancellationToken);
	    
	    UniTask<WindowMaterialData> FindWindowMaterialAsync(ulong materialId, CancellationToken cancellationToken);
	    
        TMaterial GetMaterial<TMaterial>(ulong materialId) where TMaterial : MaterialData;
    }
}