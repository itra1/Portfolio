using System;
using System.Collections.Generic;
using Core.Materials.Data;

namespace Core.Materials.Storage
{
	/// <summary>
	/// Устаревшее название - "MaterialManager"
	/// Обеспечивает только хранение материалов с соответствующим доступом к ним.
	/// </summary>
	public interface IMaterialDataStorage
	{
		IReadOnlyDictionary<string, Type> TypesByModel { get; }

		MaterialData Add(MaterialData material,
			Action<MaterialData> onAddCompleted = null);
		
		TMaterialData Add<TMaterialData>(TMaterialData material,
			Action<TMaterialData> onAddCompleted = null)
			where TMaterialData : MaterialData;
		
		IList<TMaterialData> Add<TMaterialData>(IList<TMaterialData> materials,
			Action<TMaterialData> onAddCompleted = null) 
			where TMaterialData : MaterialData;

		MaterialData Update(MaterialData material,
			Action<MaterialData> onUpdateCompleted = null);
		
		TMaterialData Update<TMaterialData>(TMaterialData material,
			Action<TMaterialData> onUpdateCompleted = null)
			where TMaterialData : MaterialData;
		
		IList<TMaterialData> Update<TMaterialData>(IList<TMaterialData> materials,
			Action<TMaterialData> onUpdateCompleted = null) 
			where TMaterialData : MaterialData;

		MaterialData UpdateOrAdd(MaterialData material,
			Action<MaterialData> onUpdateOrAddCompleted = null);

		TMaterialData UpdateOrAdd<TMaterialData>(TMaterialData material,
			Action<TMaterialData> onUpdateOrAddCompleted = null)
			where TMaterialData : MaterialData;
		
		IList<TMaterialData> UpdateOrAdd<TMaterialData>(IList<TMaterialData> materials,
			Action<TMaterialData> onUpdateOrAddCompleted = null) 
			where TMaterialData : MaterialData;
		
		void Remove(MaterialData material);
		
		void Remove<TMaterialData>(IList<TMaterialData> materials)
			where TMaterialData : MaterialData;
		
		MaterialData Get(ulong id);
		
		MaterialData Get(Type type, ulong id);
		
		TMaterialData Get<TMaterialData>(ulong id)
			where TMaterialData : MaterialData;
		
		TMaterialData GetByModelAndName<TMaterialData>(string model, string name)
			where TMaterialData : MaterialData;
		
		TMaterialData GetByModelAndTag<TMaterialData>(string model, string tag)
			where TMaterialData : MaterialData;
		
		IReadOnlyList<MaterialData> GetList();
		
		IReadOnlyList<MaterialData> GetList(Type type);
		
		IReadOnlyList<TMaterialData> GetList<TMaterialData>()
			where TMaterialData : MaterialData;
		
		IReadOnlyList<TMaterialData> GetListByModel<TMaterialData>(string model)
			where TMaterialData : MaterialData;
	}
}