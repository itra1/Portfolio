using System;

namespace Core.Elements.Widgets.Base.Attributes
{
	/// <summary>
	/// Атрибут, позволяющий фабрике "WidgetPresenterFactory" связать тип данных виджета c типом презентера
	/// для создания экземпляра презентера по типу данных виджета
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class WidgetDataTypeAttribute : Attribute
	{
		public Type Type { get; }

		public WidgetDataTypeAttribute(Type type)
		{
			Type = type;
		}
	}
}