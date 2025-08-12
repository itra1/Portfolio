using System;
using System.Collections;
using UnityEngine;

namespace it
{
	/// <summary>
	/// Аттрибут необходимости обновлять Свойство в материале
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class UpdateAttribute : System.Attribute { }

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class UpdateMaterialEventAttribute : System.Attribute	{	}
}