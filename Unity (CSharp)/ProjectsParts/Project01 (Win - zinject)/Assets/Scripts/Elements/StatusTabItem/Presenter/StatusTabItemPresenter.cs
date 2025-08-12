using Base.Presenter;
using Core.Materials.Data;
using UnityEngine;

namespace Elements.StatusTabItem.Presenter
{
	public class StatusTabItemPresenter : PresenterBase, IStatusTabItemPresenter
	{
		private ContentAreaMaterialData _areaMaterial;
		
		public ulong? AreaId => _areaMaterial?.Id;
		
		public bool SetMaterial(ContentAreaMaterialData areaMaterial)
		{
			if (areaMaterial == null)
			{
				Debug.LogError("An attempt was detected to assign a null area material to the StatusTabItemPresenter");
				return false;
			}
			
			if (_areaMaterial != null)
			{
				Debug.LogError("The area material has already been assigned before. Not allowed to reassign area material to the StatusTabItemPresenter");
				return false;
			}
			
			_areaMaterial = areaMaterial;
			
			SetName($"Tab: {areaMaterial.Id} - {areaMaterial.Name}");
			
			return true;
		}
		
		public override void Unload()
		{
			_areaMaterial = null;
			base.Unload();
		}
	}
}