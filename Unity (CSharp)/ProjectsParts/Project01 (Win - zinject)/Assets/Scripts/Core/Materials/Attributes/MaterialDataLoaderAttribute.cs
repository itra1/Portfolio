using System;

namespace Core.Materials.Attributes
{
	/// <summary>
	/// Устаревшее название - "MaterialLoaderAttribute"
	/// Атрибут, позволяющий загрузчику "MaterialDataLoader" определить список материалов, необходимый для загрузки
	/// с сервера, с возможностью указания приоритета (шага) загрузки, URL-адреса и типа сообщения, которое будет
	/// отправлено по окончанию загрузки
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class MaterialDataLoaderAttribute : Attribute
	{
		public string Url { get; }
		
		public MaterialDataLoaderAttribute(string url)
		{
			Url = url;
		}
	}
}