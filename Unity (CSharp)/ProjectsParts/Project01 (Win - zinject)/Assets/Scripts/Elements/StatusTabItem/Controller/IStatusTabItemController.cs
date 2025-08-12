using Base.Controller;
using Core.Materials.Data;
using Elements.Windows.Base;
using UnityEngine;

namespace Elements.StatusTabItem.Controller
{
	public interface IStatusTabItemController : IController, IPreloadingAsync, IWindowMaterialActionPerformer, IWindowStateProvider
	{
		ContentAreaMaterialData AreaMaterial { get; }
		
		void SetParent(RectTransform parent);
	}
}