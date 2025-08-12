using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Widgets.Map.Data.Parser
{
    public class MapTextureMaterialDataParser : ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        public IList<MaterialData> Parse(string rawData)
        {
            var materials = new List<MaterialData>();
            
            var json = JSON.ParseString(rawData);
            
            materials.Add(ParseMaterial<MapTextureMaterialData>(json));
            
            return materials.ToArray();
        }
    }
}