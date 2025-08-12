using System;

namespace Core.Materials.Attributes
{
	/// <summary>
	/// Устаревшее название - "MaterialPropertyParseAttribute"
	/// Атрибут, позволяющий парсеру "MaterialDataParserHelper" определить поля или свойства материала, в которые будет
	/// осуществлена запись в процессе разбора сырых данных по указанному ключу
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class MaterialDataPropertyParseAttribute : Attribute
	{
		public string Key { get; }
		
		public MaterialDataPropertyParseAttribute(string key = null)
		{
			Key = key;
		}
	}
}