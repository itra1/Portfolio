using System;

namespace Core.Materials.Attributes
{
	/// <summary>
	/// Устаревшее название - "UpdateAttribute"
	/// Атрибут, позволяющий обновлять поле или cвойство в материале
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class MaterialDataPropertyUpdateAttribute : Attribute
	{
		
	}
}