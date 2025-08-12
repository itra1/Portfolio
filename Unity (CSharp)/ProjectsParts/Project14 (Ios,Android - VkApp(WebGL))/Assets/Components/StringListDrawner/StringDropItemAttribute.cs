using System;
using UnityEngine;

namespace StringDrop {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class StringDropItemAttribute : PropertyAttribute {
		public int Index { get; private set; }
		public StringDropItemAttribute(int index = -1) {
			Index = index;
		}
	}
}
