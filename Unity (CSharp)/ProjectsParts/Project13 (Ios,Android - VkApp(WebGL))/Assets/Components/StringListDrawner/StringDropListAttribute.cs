using System;
using UnityEngine;

namespace StringDrop {
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class StringDropListAttribute : PropertyAttribute {
		public System.Type Type { get; private set; }
		public StringDropListAttribute(System.Type classType) {
			Type = classType;
		}
	}
}
