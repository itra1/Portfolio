using Base.Presenter;
using Core.Elements.Presentation.Data;
using UnityEngine;

namespace Elements.Presentation.Presenter
{
	public class PresentationPresenter : PresenterBase, IPresentationPresenter
	{
		private PresentationMaterialData _material;
		private PresentationAreaMaterialData _areaMaterial;
		
		public bool SetMaterials(PresentationMaterialData material, PresentationAreaMaterialData areaMaterial)
		{
			if (material == null || areaMaterial == null)
			{
				Debug.LogError("An attempt was detected to assign a null material or a null area material to the PresentationPresenter");
				return false;
			}
			
			if (_material != null || _areaMaterial != null)
			{
				Debug.LogError("Materials have already been assigned before. Not allowed to reassign material or area material in the PresentationPresenter");
				return false;
			}
			
			_material = material;
			_areaMaterial = areaMaterial;
			
			SetName($"Presentation: [{areaMaterial.Id}, {material.Id}] - {material.Name}");
			
			return true;
		}
	}
}