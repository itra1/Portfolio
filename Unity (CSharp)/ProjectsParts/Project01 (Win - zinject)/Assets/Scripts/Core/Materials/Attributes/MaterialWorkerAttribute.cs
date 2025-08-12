using System;

namespace Core.Materials.Attributes
{
	// Warning! Do not allow multiple instances of this attribute.
	// A worker is supposed to have multiple responsibilities by implementing multiple interfaces.
	[AttributeUsage(AttributeTargets.Class)]
	public class MaterialDataWorkerAttribute : Attribute
	{
		public Type Type { get; }

		public MaterialDataWorkerAttribute(Type type)
		{
			Type = type;
		}
	}
}