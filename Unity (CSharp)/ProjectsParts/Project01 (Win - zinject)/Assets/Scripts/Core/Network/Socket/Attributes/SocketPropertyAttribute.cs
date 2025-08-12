using System;

namespace Core.Network.Socket.Attributes
{
	/// <summary>
	/// Атрибут оправки события изменения состояния
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class SocketPropertyAttribute : Attribute
	{
		public string EventName { get; }
		public string Name { get; }
		public string Description  { get; }
		public bool IsSubObject  { get; }
		
		public SocketPropertyAttribute(string eventName, string name, string description = "", bool isSubObject = false)
		{
			EventName = eventName;
			Name = name;
			Description = description;
			IsSubObject = isSubObject;
		}
	}
}