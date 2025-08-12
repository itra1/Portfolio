using System;
using System.Collections.Generic;
using Core.Materials.Data;
using Leguar.TotalJSON;

namespace Core.Workers.Material.Coordinator
{
	public interface IMaterialWorkerCoordinator
	{
		void PerformActionAfterAddingToStorageOf(MaterialData material);
		
		void UpdateChildrenAt<TParentMaterialData, TChildMaterialData>(TParentMaterialData parent, IReadOnlyList<TChildMaterialData> children) 
			where TParentMaterialData : AreaMaterialData
			where TChildMaterialData : AreaMaterialData;
		
		Type DefineConcreteTypeFrom(Type materialType, JSON json);
	}
}