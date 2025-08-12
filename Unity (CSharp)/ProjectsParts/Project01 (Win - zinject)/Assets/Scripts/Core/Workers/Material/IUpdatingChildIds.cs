using System.Collections.Generic;
using Core.Materials.Data;

namespace Core.Workers.Material
{
	public interface IUpdatingChildren
	{
		void UpdateChildrenAt<TParentMaterialData, TChildMaterialData>(TParentMaterialData parent, IReadOnlyList<TChildMaterialData> children) 
			where TParentMaterialData : AreaMaterialData
			where TChildMaterialData : AreaMaterialData;
	}
}