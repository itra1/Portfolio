using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Common.Data.Parsing
{
    public class MaterialDataParser<TMaterialData> : ElementMaterialDataParserBase, IElementMaterialDataParser
        where TMaterialData : MaterialData
    {
        public IList<MaterialData> Parse(string rawData)
        {
            var materials = new List<MaterialData>
            {
                ParseMaterial<TMaterialData>(JSON.ParseString(rawData))
            };
            
            return materials.ToArray();
        }
    }
}