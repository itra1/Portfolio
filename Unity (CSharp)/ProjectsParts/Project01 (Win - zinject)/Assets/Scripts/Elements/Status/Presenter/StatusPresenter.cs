using Base.Presenter;
using Core.Elements.Status.Data;
using UnityEngine;

namespace Elements.Status.Presenter
{
	public class StatusPresenter : PresenterBase, IStatusPresenter
	{
		private StatusMaterialData _material;
		private StatusAreaMaterialData _areaMaterial;
		
		public bool SetMaterial(StatusMaterialData material, StatusAreaMaterialData areaMaterial)
		{
			if (material == null || areaMaterial == null)
			{
				Debug.LogError("An attempt was detected to assign a null material or a null area material to the StatusPresenter");
				return false;
			}
			
			if (_material != null || _areaMaterial != null)
			{
				Debug.LogError("Materials have already been assigned before. Not allowed to reassign material or area material in the StatusPresenter");
				return false;
			}
			
			_material = material;
			_areaMaterial = areaMaterial;
			
			SetName($"Status: [{areaMaterial.Id}, {material.Id}] - {material.Name}");
			
			return true;
		}
	}
}