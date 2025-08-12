using System.Collections.Generic;
using Core.Materials.Data;
using Elements.Windows.Base;

namespace Elements.StatusTabs.Controller
{
    public interface IStatusTabsActionPerformer : IWindowMaterialActionPerformer
    {
        IReadOnlyList<ContentAreaMaterialData> AreaMaterials { get; }
        ContentAreaMaterialData ActiveAreaMaterial { get; }
    }
}