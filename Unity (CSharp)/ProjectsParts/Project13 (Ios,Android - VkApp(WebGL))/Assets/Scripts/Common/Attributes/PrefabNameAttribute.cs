using System;

namespace Game.Common.Attributes {
	[AttributeUsage(AttributeTargets.Class)]
	public class PrefabNameAttribute :System.Attribute, IPrefabName {
		public string Name { get; private set; }

		string IPrefabName.PrefabName => Name;

		public PrefabNameAttribute(string name) {
			Name = name;
		}

	}
}
