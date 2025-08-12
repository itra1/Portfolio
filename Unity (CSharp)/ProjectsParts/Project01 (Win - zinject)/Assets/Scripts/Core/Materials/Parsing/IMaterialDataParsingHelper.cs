using System;
using Leguar.TotalJSON;

namespace Core.Materials.Parsing
{
	/// <summary>
	/// Устаревшее название - "ParserHelper"
	/// </summary>
	public interface IMaterialDataParsingHelper
	{
		TMaterialData Parse<TMaterialData>(string rawData) where TMaterialData : class;
		TMaterialData Parse<TMaterialData>(JValue jValue) where TMaterialData : class;
		object Parse(Type type, JValue jValue);
		TMaterialData ParseClass<TMaterialData>(JSON json) where TMaterialData : class;
		object ParseClass(Type type, JSON json);
	}
}