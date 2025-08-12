using System;

namespace Core.Materials.Attributes
{
	/// <summary>
	/// Атрибут, позволяющий хранилищу "MaterialDataStorage" создать словарь типов данных материала по уникальному
	/// значению его модели в качестве ключа
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class MaterialModelAttribute : Attribute
	{
		public string Model { get; }
		
		public MaterialModelAttribute(string model)
		{
			Model = model;
		}
	}
}