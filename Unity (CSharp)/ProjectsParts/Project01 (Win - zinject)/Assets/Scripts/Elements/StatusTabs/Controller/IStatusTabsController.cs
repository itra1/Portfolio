using Base.Controller;
using Core.Elements.StatusColumn.Data;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using Elements.StatusTabItem.Controller;

namespace Elements.StatusTabs.Controller
{
	public interface IStatusTabsController : IController, IPreloadingAsync, IStatusTabsActionPerformer, IStatusTabsEventDispatcher
	{
		StatusContentAreaMaterialData AreaMaterial { get; }

		int ChildrenCount { get; }
		
		bool AddChild(IStatusTabItemController child);
		UniTask<bool> AddChildAsync(ContentAreaMaterialData areaMaterial);
		bool ContainsChild(ContentAreaMaterialData areaMaterial);
		void SetChildActive(ulong areaMaterialId);
		bool RemoveChild(ContentAreaMaterialData areaMaterial, out IStatusTabItemController child);
		bool RemoveChild(ContentAreaMaterialData areaMaterial);
	}
}