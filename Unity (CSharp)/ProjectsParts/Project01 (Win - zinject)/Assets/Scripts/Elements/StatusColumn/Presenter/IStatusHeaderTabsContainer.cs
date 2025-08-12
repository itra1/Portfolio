using System.Collections.Generic;
using Core.Elements.Windows.Base.Data;

namespace Elements.StatusColumn.Presenter
{
    public interface IStatusHeaderTabsContainer
    {
        int TabsCount { get; }
        
        bool ContainsTabInHeader(WindowMaterialData material);
        bool AddTabToHeader(WindowMaterialData material);
        void ActivateTabInHeader(WindowMaterialData material);
        void ReorderTabsInHeader(IReadOnlyList<WindowMaterialData> orderedMaterials);
        bool RemoveTabFromHeader(ulong materialId);
    }
}