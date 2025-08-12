using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Core.Materials.Data;

namespace Core.Materials.Storage.Utils
{
	public static class MaterialDataStorageExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsByIdAndModel(this IReadOnlyList<MaterialData> materials, ulong id, string model)
		{
			var materialsCount = materials.Count;
			
			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
				
				if (m.Id == id && m.Model == model)
					return true;
			}

			return false;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MaterialData GetById(this IReadOnlyList<MaterialData> materials, ulong id)
		{
			var materialsCount = materials.Count;
			
			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
					
				if (m.Id == id)
					return m;
			}

			return null;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetById(this IReadOnlyList<MaterialData> materials, ulong id, out MaterialData material)
		{
			var materialsCount = materials.Count;
			
			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
				
				if (m.Id != id)
					continue;
				
				material = m;
				return true;
			}

			material = null;
			return false;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MaterialData GetById(this IReadOnlyDictionary<Type, List<MaterialData>> materialsByType, Type type, ulong id)
		{
			foreach (var (t, materials) in materialsByType)
			{
				if (type.IsAssignableFrom(t) && materials.TryGetById(id, out var material))
					return material;
			}

			return null;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MaterialData GetByModelAndName(this IReadOnlyList<MaterialData> materials, string model, string name)
		{
			var materialsCount = materials.Count;
			
			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
					
				if (m.Model == model && m.Name == name)
					return m;
			}

			return null;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetByModelAndName(this IReadOnlyList<MaterialData> materials, string model, string name, out MaterialData material)
		{
			var materialsCount = materials.Count;
			
			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
				
				if (m.Model != model || m.Name != name)
					continue;
				
				material = m;
				return true;
			}

			material = null;
			return false;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MaterialData GetByModelAndName(this IReadOnlyDictionary<Type, List<MaterialData>> materialsByType, Type type, string model, string name)
		{
			foreach (var (t, materials) in materialsByType)
			{
				if (type.IsAssignableFrom(t) && materials.TryGetByModelAndName(model, name, out var material))
					return material;
			}
			
			return null;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MaterialData GetByModelAndTag(this IReadOnlyList<MaterialData> materials, string model, string tag)
		{
			var materialsCount = materials.Count;
			
			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
				
				if (m.Model != model)
					continue;
				
				var tagMaterials = m.Tags;
				
				if (tagMaterials == null)
					continue;
				
				var tagMaterialsCount = tagMaterials.Count;
				
				for (var j = 0; j < tagMaterialsCount; j++)
				{
					var tm = tagMaterials[i];
					
					if (tm.Name != tag)
						continue;
						
					return m;
				}
			}

			return null;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetByModelAndTag(this IReadOnlyList<MaterialData> materials, string model, string tag, out MaterialData material)
		{
			var materialsCount = materials.Count;
			
			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
				
				if (m.Model != model)
					continue;
				
				var tagMaterials = m.Tags;
				
				if (tagMaterials == null)
					continue;
				
				var tagMaterialsCount = tagMaterials.Count;
				
				for (var j = 0; j < tagMaterialsCount; j++)
				{
					var tm = tagMaterials[i];
					
					if (tm.Name != tag)
						continue;
						
					material = m;
					return true;
				}
			}

			material = null;
			return false;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MaterialData GetByModelAndTag(this IReadOnlyDictionary<Type, List<MaterialData>> materialsByType, Type type, string model, string tag)
		{
			foreach (var (t, materials) in materialsByType)
			{
				if (type.IsAssignableFrom(t) && materials.TryGetByModelAndTag(model, tag, out var material))
					return material;
			}
			
			return null;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<TMaterialData> GetList<TMaterialData>(this IReadOnlyList<MaterialData> materials)
			where TMaterialData : MaterialData
		{
			var materialsCount = materials.Count;
			var result = new TMaterialData[materialsCount];
			
			for (var i = 0; i < materialsCount; i++)
				result[i] = (TMaterialData) materials[i];
			
			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<MaterialData> GetList(this IReadOnlyDictionary<Type, List<MaterialData>> materialsByType)
		{
			var result = new List<MaterialData>();

			foreach (var materials in materialsByType.Values)
			{
				var materialsCount = materials.Count;
				
				for (var i = 0; i < materialsCount; i++)
					result.Add(materials[i]);
			}
			
			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<MaterialData> GetList(this IReadOnlyDictionary<Type, List<MaterialData>> materialsByType, Type type)
		{
			var result = new List<MaterialData>();

			foreach (var (t, materials) in materialsByType)
			{
				if (!type.IsAssignableFrom(t)) 
					continue;
				
				var materialsCount = materials.Count;
				
				for (var i = 0; i < materialsCount; i++)
					result.Add(materials[i]);
			}
			
			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<TMaterialData> GetList<TMaterialData>(this IReadOnlyDictionary<Type, List<MaterialData>> materialsByType, Type type) 
			where TMaterialData : MaterialData
		{
			var result = new List<TMaterialData>();

			foreach (var (t, materials) in materialsByType)
			{
				if (!type.IsAssignableFrom(t)) 
					continue;
				
				var materialsCount = materials.Count;
				
				for (var i = 0; i < materialsCount; i++)
					result.Add((TMaterialData) materials[i]);
			}
			
			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<TMaterialData> GetListByModel<TMaterialData>(this IReadOnlyList<MaterialData> materials, string model)
			where TMaterialData : MaterialData
		{
			var materialsCount = materials.Count;
			var result = new List<TMaterialData>();

			for (var i = 0; i < materialsCount; i++)
			{
				var m = materials[i];
				
				if (m.Model != model)
					continue;
				
				result.Add((TMaterialData) m);
			}
			
			return result;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<TMaterialData> GetListByModel<TMaterialData>(this IReadOnlyDictionary<Type, List<MaterialData>> materialsByType, string model) 
			where TMaterialData : MaterialData
		{
			var type = typeof(TMaterialData);
			var result = new List<TMaterialData>();

			foreach (var (t, materials) in materialsByType)
			{
				if (!type.IsAssignableFrom(t)) 
					continue;
				
				var materialsCount = materials.Count;

				for (var i = 0; i < materialsCount; i++)
				{
					var m = materials[i];
				
					if (m.Model != model)
						continue;
					
					result.Add((TMaterialData) m);
				}
			}
			
			return result;
		}
	}
}