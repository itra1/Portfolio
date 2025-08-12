using System;
using UnityEngine;

namespace itra.Attributes
{
	public class RequiredInterface : PropertyAttribute
	{
		public readonly Type RequireType;

		public RequiredInterface(Type requireType)
		{
			RequireType = requireType;
		}
	}
}
