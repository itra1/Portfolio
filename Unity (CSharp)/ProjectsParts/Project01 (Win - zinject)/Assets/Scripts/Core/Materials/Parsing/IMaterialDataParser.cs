using System;
using System.Collections.Generic;
using Core.Materials.Data;
using Core.Materials.Loading.AutoPreloader.Info;
using Core.Materials.Loading.Loader.Info;
using Leguar.TotalJSON;

namespace Core.Materials.Parsing
{
	/// <summary>
	/// Устаревшее название - "MaterialManager"
	/// Обеспечивает только парсинг материалов
	/// </summary>
	public interface IMaterialDataParser
	{
		void Parse(MaterialDataTypeLoadingInfo info, string rawData);
		IList<MaterialData> Parse(MaterialDataLoadingInfo info, string rawData);
		MaterialData ParseOne(Type type, string rawData);
		MaterialData ParseOne(Type type, JSON json);
		MaterialData ParseOne<TMaterialData>(string rawData) where TMaterialData : MaterialData;
		TMaterialData ParseOne<TMaterialData>(JSON json) where TMaterialData : MaterialData;
		MaterialData ParseOne(JSON json);
	}
}