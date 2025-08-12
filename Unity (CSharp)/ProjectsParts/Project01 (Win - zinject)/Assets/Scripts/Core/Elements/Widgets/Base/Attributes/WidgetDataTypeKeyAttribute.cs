using System;

namespace Core.Elements.Widgets.Base.Attributes
{
	/// <summary>
	/// Атрибут, позволяющий воркеру "WidgetMaterialDataWorker" связать значение ключа типа данных виджета
	/// c типом данных виджета для создания экземпляра данных виджета
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class WidgetDataTypeKeyAttribute : Attribute
	{
		public string Key { get; }
		
		public WidgetDataTypeKeyAttribute(string key)
		{
			Key = key;
		}
	}
}