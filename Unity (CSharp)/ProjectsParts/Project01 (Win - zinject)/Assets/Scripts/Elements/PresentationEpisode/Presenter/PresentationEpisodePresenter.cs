using Base.Presenter;
using Core.Elements.PresentationEpisode.Data;
using UnityEngine;

namespace Elements.PresentationEpisode.Presenter
{
	public class PresentationEpisodePresenter : PresenterBase, IPresentationEpisodePresenter
	{
		private PresentationEpisodeMaterialData _material;
		private PresentationEpisodeAreaMaterialData _areaMaterial;
		
		public bool SetMaterials(PresentationEpisodeMaterialData material, PresentationEpisodeAreaMaterialData areaMaterial)
		{
			if (material == null || areaMaterial == null)
			{
				Debug.LogError("An attempt was detected to assign a null material or a null area material to the PresentationEpisodePresenter");
				return false;
			}
			
			if (_material != null || _areaMaterial != null)
			{
				Debug.LogError("Materials have already been assigned before. Not allowed to reassign material or area material in the PresentationEpisodePresenter");
				return false;
			}
			
			_material = material;
			_areaMaterial = areaMaterial;
			
			SetName($"Episode: [{areaMaterial.Id}, {material.Id}] - {material.Name}");
			
			return true;
		}
	}
}