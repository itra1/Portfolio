using System;
using System.Collections;
using UnityEngine;

namespace it.Attributes
{

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SocketMessageAttribute : System.Attribute
	{
		public string AliasName;
		public string Name;

		public SocketMessageAttribute(string AliasName, string name = null)
		{
			this.AliasName = AliasName;
			this.Name = name;
		}

	}

	/// <summary>
	/// Обправка события изменения состояния
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class SocketPropertyAttribute : System.Attribute
	{
		// Имя свойства отправляемого на сервер
		public string Name;
		// Имя свойства отправляемого на сервер
		public string EventName;
		// Описание свойства
		public string Description;
		// Подобджект
		public bool IsSubObject;
		public SocketPropertyAttribute(string socket, string name, string description = "", bool isSubObject = false)
		{
			this.Name = name;
			this.EventName = socket;
			this.Description = description;
			this.IsSubObject = isSubObject;
		}

	}
}