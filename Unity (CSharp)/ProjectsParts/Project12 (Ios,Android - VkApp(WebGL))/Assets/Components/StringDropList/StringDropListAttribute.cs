using System;
using UnityEngine;

namespace StringDrop
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class StringDropListAttribute : PropertyAttribute
	{
		public System.Type Type { get; private set; }
		public bool CheckAttribute { get; private set; }

		public StringDropListAttribute(System.Type classType, bool checkAttribute = true)
		{
			Type = classType;
			CheckAttribute = checkAttribute;
		}
	}
}
