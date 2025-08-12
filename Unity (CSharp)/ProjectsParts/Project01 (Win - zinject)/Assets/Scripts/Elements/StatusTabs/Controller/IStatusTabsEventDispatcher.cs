using System;
using Core.Materials.Data;

namespace Elements.StatusTabs.Controller
{
    public interface IStatusTabsEventDispatcher
    {
        event Action<ContentAreaMaterialData> StatusTabActive;
    }
}