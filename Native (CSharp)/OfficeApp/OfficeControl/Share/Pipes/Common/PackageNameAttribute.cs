using System;

namespace OfficeControl.Pipes.Common
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PackageNameAttribute :Attribute
	{
		public string Name;

		public PackageNameAttribute(string name)
		{
			Name = name;
		}
	}
}
