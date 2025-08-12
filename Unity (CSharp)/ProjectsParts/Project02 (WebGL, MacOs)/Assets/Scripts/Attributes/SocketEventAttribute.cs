using System;
using System.Collections;
using UnityEngine;

namespace it
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SocketEventAttribute : System.Attribute
	{
		//Имя исходящего пакета
		public string Name;
		// Описание исходящщего пакета
		public string Description;
		public SocketEventAttribute(string name, string description = "")
		{
			this.Name = name;
			this.Description = description;
		}
	}
}