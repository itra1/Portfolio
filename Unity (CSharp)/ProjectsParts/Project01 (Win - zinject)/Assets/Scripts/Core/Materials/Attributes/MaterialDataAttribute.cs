using System;

namespace Core.Materials.Attributes
{
	/// <summary>
	/// Атрибут, позволяющий фабрике "MaterialPresenterFactory" связать тип материала c типом презентера
	/// для создания экземпляра презентера по типу материала
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class MaterialDataAttribute : Attribute
	{
		public Type Type { get; }
		
		public MaterialDataAttribute(Type type)
		{
			Type = type;
		}
	}
}