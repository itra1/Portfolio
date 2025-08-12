using System;

namespace itra.Attributes {
	[AttributeUsage(AttributeTargets.Class)]
	public class PrefabNameAttribute : System.Attribute {
		public string Name { get; private set; }

		public PrefabNameAttribute(string name) {
			Name = name;
		}
	}
}
